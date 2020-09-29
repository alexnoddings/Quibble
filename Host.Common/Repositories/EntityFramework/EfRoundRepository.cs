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
    internal class EfRoundRepository : IRoundRepository
    {
        private IQuibbleDbContext DbContext { get; }

        public EfRoundRepository(IQuibbleDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private IQueryable<DbRound> Entities =>
            DbContext.Rounds
                .Include(r => r.Quiz)
                .Include(r => r.Questions)
                    .ThenInclude(q => q.Answers)
                        .ThenInclude(a => a.Participant);

        public async Task<Guid> CreateAsync(DbRound round)
        {
            Ensure.NotNullOrDefault(round, nameof(round));
            if (round.Id != default)
                throw ThrowHelper.BadArgument(nameof(round), $"{nameof(round.Id)} must not be set prior to creation.");
            Ensure.NotNullOrDefault(round.QuizId, nameof(round.QuizId), $"{nameof(round.QuizId)} is not set.");

            DbContext.Rounds.Add(round);
            await DbContext.SaveChangesAsync();
            return round.Id;
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            return DbContext.Rounds.ExistsAsync(id);
        }

        public Task<DbRound> GetAsync(Guid id) => GetAsync(id, Entities);

        private Task<DbRound> GetWithoutIncludesAsync(Guid id) => GetAsync(id, DbContext.Rounds);

        private static async Task<DbRound> GetAsync(Guid id, IQueryable<DbRound> source)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            DbRound? round = await source.WithIdAsync(id);
            Ensure.Found(round, "Round", id);
            return round;
        }

        public async Task<List<DbRound>> GetForQuizAsync(Guid quizId)
        {
            if (!await DbContext.Quizzes.ExistsAsync(quizId))
                throw ThrowHelper.NotFound("Quiz", quizId);
            return await Entities.Where(r => r.QuizId == quizId).ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            DbRound round = await GetWithoutIncludesAsync(id);
            DbContext.Rounds.Remove(round);
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateTitleAsync(Guid id, string newTitle)
        {
            DbRound round = await GetWithoutIncludesAsync(id);
            round.Title = newTitle;
            await DbContext.SaveChangesAsync();
        }
    }
}
