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
    internal class EfQuestionRepository : IQuestionRepository
    {
        private IQuibbleDbContext DbContext { get; }

        public EfQuestionRepository(IQuibbleDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private IQueryable<DbQuestion> Entities =>
            DbContext.Questions
                .Include(q => q.Round)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.Participant);

        public async Task<Guid> CreateAsync(DbQuestion question)
        {
            Ensure.NotNullOrDefault(question, nameof(question));
            if (question.Id == default)
                throw ThrowHelper.BadArgument(nameof(question), $"{nameof(question.Id)} must not be set prior to creation.");
            Ensure.NotNullOrDefault(question.RoundId, nameof(question.RoundId), $"{nameof(question.RoundId)} is not set.");

            DbContext.Questions.Add(question);
            await DbContext.SaveChangesAsync();
            return question.Id;
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            return DbContext.Questions.ExistsAsync(id);
        }

        public Task<DbQuestion> GetAsync(Guid id) => GetAsync(id, Entities);

        private Task<DbQuestion> GetWithoutIncludesAsync(Guid id) => GetAsync(id, DbContext.Questions);

        private static async Task<DbQuestion> GetAsync(Guid id, IQueryable<DbQuestion> source)
        {
            Ensure.NotNullOrDefault(id, nameof(id));
            DbQuestion? question = await source.WithIdAsync(id);
            Ensure.Found(question, "Question", id);
            return question;
        }

        public async Task<List<DbQuestion>> GetForQuizAsync(Guid quizId)
        {
            if (!await DbContext.Quizzes.ExistsAsync(quizId))
                throw ThrowHelper.NotFound("Quiz", quizId);
            var questions =
                from question in DbContext.Questions
                join round in DbContext.Rounds on question.RoundId equals round.Id
                where round.QuizId == quizId
                select question;
            return await questions.ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            DbQuestion question = await GetWithoutIncludesAsync(id);
            DbContext.Questions.Remove(question);
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateTextAsync(Guid id, string newText)
        {
            DbQuestion question = await GetWithoutIncludesAsync(id);
            question.QuestionText = newText;
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateAnswerAsync(Guid id, string newAnswer)
        {
            DbQuestion question = await GetWithoutIncludesAsync(id);
            question.CorrectAnswer = newAnswer;
            await DbContext.SaveChangesAsync();
        }
    }
}
