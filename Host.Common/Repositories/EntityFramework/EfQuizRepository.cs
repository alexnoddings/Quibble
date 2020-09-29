using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quibble.Host.Common.Data;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;

namespace Quibble.Host.Common.Repositories.EntityFramework
{
    internal class EfQuizRepository : IQuizRepository
    {
        private IQuibbleDbContext DbContext { get; }

        public EfQuizRepository(IQuibbleDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private IQueryable<DbQuiz> Entities =>
            DbContext.Quizzes
                .Include(q => q.Participants)
                    .ThenInclude(p => p.Answers)
                .Include(q => q.Rounds)
                    .ThenInclude(r => r.Questions)
                        .ThenInclude(q => q.Answers)
                            .ThenInclude(a => a.Participant);

        public async Task<Guid> CreateAsync(DbQuiz quiz)
        {
            Ensure.NotNullOrDefault(quiz, nameof(quiz));
            if (quiz.Id != default)
                throw ThrowHelper.BadArgument(nameof(quiz), $"{nameof(quiz.Id)} must not be set prior to creation.");

            DbContext.Quizzes.Add(quiz);
            await DbContext.SaveChangesAsync();
            return quiz.Id;
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            return DbContext.Quizzes.AnyAsync(q => q.Id == id);
        }

        public Task<DbQuiz> GetAsync(Guid id) => GetAsync(id, Entities);

        private Task<DbQuiz> GetWithoutIncludesAsync(Guid id) => GetAsync(id, DbContext.Quizzes);

        private static async Task<DbQuiz> GetAsync(Guid id, IQueryable<DbQuiz> source)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            DbQuiz? quiz = await source.WithIdAsync(id);
            Ensure.Found(quiz, "Quiz", id);
            return quiz;
        }

        public async Task DeleteAsync(Guid id)
        {
            var quiz = await GetWithoutIncludesAsync(id);
            DbContext.Quizzes.Remove(quiz);
            await DbContext.SaveChangesAsync();
        }

        public async Task PublishAsync(Guid id)
        {
            var quiz = await GetWithoutIncludesAsync(id);
            quiz.PublishedAt = DateTime.UtcNow;
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateTitleAsync(Guid id, string newTitle)
        {
            var quiz = await GetWithoutIncludesAsync(id);
            quiz.Title = newTitle;
            await DbContext.SaveChangesAsync();
        }
    }
}
