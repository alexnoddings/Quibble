using Microsoft.EntityFrameworkCore;
using Quibble.Core.Entities;
using Quibble.Host.Common.Data;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quibble.Host.Common.Repositories.EntityFramework
{
    internal class EfAnswerRepository : IAnswerRepository
    {
        private IQuibbleDbContext DbContext { get; }

        public EfAnswerRepository(IQuibbleDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private IQueryable<DbParticipantAnswer> Entities =>
            DbContext.ParticipantAnswers
                .Include(a => a.Participant)
                .Include(a => a.Question);

        public Task<DbParticipantAnswer> GetAsync(Guid id) => GetAsync(id, Entities);

        private Task<DbParticipantAnswer> GetWithoutIncludesAsync(Guid id) => GetAsync(id, DbContext.ParticipantAnswers);

        private static async Task<DbParticipantAnswer> GetAsync(Guid id, IQueryable<DbParticipantAnswer> source)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            DbParticipantAnswer? answer = await source.WithIdAsync(id);
            Ensure.Found(answer, "Answer", id);
            return answer;
        }

        public async Task<List<DbParticipantAnswer>> GetForQuizAsync(Guid quizId)
        {
            if (!await DbContext.Quizzes.ExistsAsync(quizId))
                throw ThrowHelper.NotFound("Quiz", quizId);
            var answers =
                from answer in DbContext.ParticipantAnswers
                join question in DbContext.Questions on answer.QuestionId equals question.Id
                join round in DbContext.Rounds on question.RoundId equals round.Id
                where round.QuizId == quizId
                select answer;
            return await answers.ToListAsync();
        }

        public async Task<List<DbParticipantAnswer>> GetForQuizAndUserAsync(Guid quizId, Guid userId)
        {
            if (!await DbContext.Quizzes.ExistsAsync(quizId))
                throw ThrowHelper.NotFound("Quiz", quizId);
            if (!await DbContext.Users.ExistsAsync(userId))
                throw ThrowHelper.NotFound("User", userId);

            var answers =
                from answer in DbContext.ParticipantAnswers
                join participant in DbContext.Participants on answer.ParticipantId equals participant.Id
                join question in DbContext.Questions on answer.QuestionId equals question.Id
                join round in DbContext.Rounds on question.RoundId equals round.Id
                where round.QuizId == quizId && participant.UserId == userId
                select answer;
            return await answers.ToListAsync();
        }

        public async Task UpdateAnswerMarkAsync(Guid id, AnswerMark newMark)
        {
            DbParticipantAnswer answer = await GetWithoutIncludesAsync(id);
            answer.Mark = newMark;
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateAnswerTextAsync(Guid id, string newText)
        {
            DbParticipantAnswer answer = await GetWithoutIncludesAsync(id);
            answer.Text = newText;
            await DbContext.SaveChangesAsync();
        }
    }
}
