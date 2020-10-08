using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quibble.Host.Common.Data;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;

namespace Quibble.Host.Common.Repositories.EntityFramework
{
    internal class EfParticipantRepository : IParticipantRepository
    {
        private IQuibbleDbContext DbContext { get; }

        public EfParticipantRepository(IQuibbleDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private IQueryable<DbParticipant> Entities =>
            DbContext.Participants
                .Include(p => p.Quiz)
                .Include(p => p.User)
                .Include(p => p.Answers);

        public async Task<Guid> CreateAsync(DbParticipant participant)
        {
            Ensure.NotNullOrDefault(participant, nameof(participant));
            if (participant.Id != default)
                throw ThrowHelper.BadArgument(nameof(participant), $"{nameof(participant.Id)} must not be set prior to creation.");
            Ensure.NotNullOrDefault(participant.QuizId, nameof(participant.QuizId), $"{nameof(participant.QuizId)} is not set.");
            Ensure.NotNullOrDefault(participant.UserId, nameof(participant.UserId), $"{nameof(participant.UserId)} is not set.");

            DbContext.Participants.Add(participant);
            await DbContext.SaveChangesAsync();

            var questionIds = from q in DbContext.Questions
                join r in DbContext.Rounds on q.RoundId equals r.Id
                where r.QuizId == participant.QuizId
                select q.Id;
            var answers = await questionIds.Select(q => new DbParticipantAnswer {ParticipantId = participant.Id, QuestionId = q}).ToListAsync();
            DbContext.ParticipantAnswers.AddRange(answers);
            await DbContext.SaveChangesAsync();

            return participant.Id;
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            return DbContext.Participants.ExistsAsync(id);
        }

        public Task<bool> IsParticipatingAsync(Guid quizId, Guid userId)
        {
            Ensure.NotNullOrDefault(quizId, nameof(quizId));
            Ensure.NotNullOrDefault(userId, nameof(userId));
            return DbContext.Participants.AnyAsync(p => p.QuizId == quizId && p.UserId == userId);
        }

        public async Task<DbParticipant> GetAsync(Guid quizId, Guid userId)
        {
            Ensure.NotNullOrDefault(quizId, nameof(quizId));
            Ensure.NotNullOrDefault(userId, nameof(userId));
            DbParticipant? participant = await Entities.Where(p => p.QuizId == quizId && p.UserId == userId).FirstOrDefaultAsync();
            Ensure.Found(participant, "Participant", $"{quizId} + {userId}");
            return participant;
        }

        public Task<DbParticipant> GetAsync(Guid id) => GetAsync(id, Entities);

        private Task<DbParticipant> GetWithoutIncludesAsync(Guid id) => GetAsync(id, DbContext.Participants);

        private static async Task<DbParticipant> GetAsync(Guid id, IQueryable<DbParticipant> source)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            DbParticipant? participant = await source.WithIdAsync(id);
            Ensure.Found(participant, "Participant", id);
            return participant;
        }

        public async Task<List<DbParticipant>> GetForQuizAsync(Guid quizId)
        {
            if (!await DbContext.Quizzes.ExistsAsync(quizId))
                throw ThrowHelper.NotFound("Quiz", quizId);
            return await Entities.Where(p => p.QuizId == quizId).ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            DbParticipant participant = await GetWithoutIncludesAsync(id);
            List<DbParticipantAnswer> answers = await DbContext.ParticipantAnswers.Where(a => a.ParticipantId == id).ToListAsync();
            DbContext.ParticipantAnswers.RemoveRange(answers);
            DbContext.Participants.Remove(participant);
            await DbContext.SaveChangesAsync();
        }
    }
}
