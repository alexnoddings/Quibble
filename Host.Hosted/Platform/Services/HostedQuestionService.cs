using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Hosted.Platform.Events;
using Quibble.UI.Core.Entities;
using Quibble.UI.Core.Services;

namespace Quibble.Host.Hosted.Platform.Services
{
    public class HostedQuestionService : HostedServiceBase, IQuestionService
    {
        private IQuestionEventsInvoker QuestionEvents { get; }

        public HostedQuestionService(IServiceProvider serviceProvider, IQuestionEventsInvoker questionEventsInvoker)
            : base(serviceProvider)
        {
            QuestionEvents = questionEventsInvoker;
        }

        public async Task CreateAsync(Guid parentRoundId)
        {
            DbRound? round = await DbContext.Rounds.FindAsync(parentRoundId);
            if (round == null)
                throw ThrowHelper.NotFound("Round", parentRoundId);

            var question = new DbQuestion {RoundId = round.Id};
            DbContext.Questions.Add(question);
            await DbContext.SaveChangesAsync();
            await QuestionEvents.InvokeQuestionAddedAsync(question);
        }

        public async Task<List<DtoQuestion>> GetForQuizAsync(Guid quizId)
        {
            DbQuiz? quiz = await DbContext.Quizzes.FindAsync(quizId);
            if (quiz == null)
                throw ThrowHelper.NotFound("Quiz", quizId);

            var questions =
                from question in DbContext.Questions
                join round in DbContext.Rounds on question.RoundId equals round.Id
                where round.QuizId == quizId
                select new DtoQuestion(question);
            return await questions.ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            DbQuestion? question = await DbContext.Questions.FindAsync(id);
            if (question == null)
                throw ThrowHelper.NotFound("Question", id);

            DbContext.Questions.Remove(question);
            await DbContext.SaveChangesAsync();
            await QuestionEvents.InvokeQuestionDeletedAsync(id);
        }

        public async Task UpdateTextAsync(Guid id, string newText)
        {
            newText ??= string.Empty;

            DbQuestion? question = await DbContext.Questions.FindAsync(id);
            if (question == null)
                throw ThrowHelper.NotFound("Question", id);
            question.QuestionText = newText;

            await DbContext.SaveChangesAsync();
            await QuestionEvents.InvokeTextUpdatedAsync(id, newText);
        }

        public async Task UpdateAnswerAsync(Guid id, string newAnswer)
        {
            newAnswer ??= string.Empty;

            DbQuestion? question = await DbContext.Questions.FindAsync(id);
            if (question == null)
                throw ThrowHelper.NotFound("Question", id);
            question.CorrectAnswer = newAnswer;

            await DbContext.SaveChangesAsync();
            await QuestionEvents.InvokeAnswerUpdatedAsync(id, newAnswer);
        }
    }
}
