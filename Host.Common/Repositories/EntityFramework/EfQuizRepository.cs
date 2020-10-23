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

        public async Task<DbQuiz> CreateAsync(DbQuiz quiz)
        {
            Ensure.NotNullOrDefault(quiz, nameof(quiz));
            if (quiz.Id != default)
                throw ThrowHelper.BadArgument(nameof(quiz), $"{nameof(quiz.Id)} must not be set prior to creation.");

            DbContext.Quizzes.Add(quiz);
            await DbContext.SaveChangesAsync();
            return quiz;
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

        public async Task<DateTime> PublishAsync(Guid id)
        {
            var quiz = await GetWithoutIncludesAsync(id);

            DateTime now = DateTime.UtcNow;
            quiz.PublishedAt = now;

            // Empty rounds are deleted when publishing
            IQueryable<DbRound> emptyRounds =
                from r in DbContext.Rounds
                where r.QuizId == id
                where r.Questions.Count == 0
                select r;
            DbContext.Rounds.RemoveRange(emptyRounds);

            await DbContext.SaveChangesAsync();
            return now;
        }

        public async Task UpdateTitleAsync(Guid id, string newTitle)
        {
            var quiz = await GetWithoutIncludesAsync(id);
            quiz.Title = newTitle;
            await DbContext.SaveChangesAsync();
        }

        public async Task<List<(string, Guid)>> GetQuizzesByUserAsync(Guid userId)
        {
            Ensure.NotNullOrDefault(userId, nameof(userId));
            DbQuibbleUser? user = await DbContext.Users.WithIdAsync(userId);
            Ensure.Found(user, "User", userId);
            IQueryable<DbQuiz> quizzes =
                from q in DbContext.Quizzes
                where q.OwnerId == userId
                select q;
            return (await quizzes.ToListAsync()).ConvertAll(q => (q.Title, q.Id));
        }

        public async Task<List<(string, Guid)>> GetQuizzesParticipatedInAsync(Guid userId)
        {
            Ensure.NotNullOrDefault(userId, nameof(userId));
            DbQuibbleUser? user = await DbContext.Users.WithIdAsync(userId);
            Ensure.Found(user, "User", userId);
            IQueryable<DbQuiz> quizzes =
                from p in DbContext.Participants
                join q in DbContext.Quizzes
                    on p.QuizId equals q.Id
                where p.UserId == userId
                select q;
            return (await quizzes.ToListAsync()).ConvertAll(q => (q.Title, q.Id));
        }
    }
}
