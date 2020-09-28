using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Hosted.Platform.Events;
using Quibble.UI.Core.Entities;
using Quibble.UI.Core.Services;

namespace Quibble.Host.Hosted.Platform.Services
{
    [Authorize]
    public class HostedQuizService : HostedServiceBase, IQuizService
    {
        private IQuizEventsInvoker QuizEvents { get; }

        public HostedQuizService(IServiceProvider serviceProvider, IQuizEventsInvoker quizEventsInvoker)
            : base(serviceProvider)
        {
            QuizEvents = quizEventsInvoker;
        }

        public async Task<Guid> CreateAsync(string title)
        {
            DbQuibbleUser user = await GetCurrentUserAsync();
            var quiz = new DbQuiz
            {
                OwnerId = user.Id,
                Title = title ?? string.Empty
            };
            DbContext.Quizzes.Add(quiz);
            await DbContext.SaveChangesAsync();

            return quiz.Id;
        }

        public async Task<DtoQuiz> GetAsync(Guid id)
        {
            DbQuiz? dbQuiz = await DbContext.Quizzes.FindAsync(id);
            if (dbQuiz == null)
                throw ThrowHelper.NotFound("Quiz", id);

            if (dbQuiz.PublishedAt == null)
            {
                DbQuibbleUser user = await GetCurrentUserAsync();
                if (user.Id != dbQuiz.OwnerId)
                    throw ThrowHelper.Unauthorised("You may not view this quiz until it is published.");
            }

            return new DtoQuiz(dbQuiz);
        }

        public async Task PublishAsync(Guid id)
        {
            DbQuiz? quiz = await DbContext.Quizzes.FindAsync(id);
            if (quiz == null)
                throw ThrowHelper.NotFound("Quiz", id);

            if (quiz.PublishedAt != null)
                throw ThrowHelper.InvalidOperation("The quiz is already published.");

            DbQuibbleUser user = await GetCurrentUserAsync();
            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised("You are not the quiz owner.");

            DateTime now = DateTime.UtcNow;
            quiz.PublishedAt = now;
            await DbContext.SaveChangesAsync();
            await QuizEvents.InvokePublishedAsync(id, now);
        }

        public async Task DeleteAsync(Guid id)
        {
            DbQuiz? quiz = await DbContext.Quizzes.FindAsync(id);
            if (quiz == null)
                throw ThrowHelper.NotFound("Quiz", id);

            DbQuibbleUser user = await GetCurrentUserAsync();
            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised("You are not the quiz owner.");

            DbContext.Quizzes.Remove(quiz);
            await DbContext.SaveChangesAsync();
            await QuizEvents.InvokeDeletedAsync(id);
        }

        public async Task UpdateTitleAsync(Guid id, string newTitle)
        {
            newTitle ??= string.Empty;

            DbQuiz? quiz = await DbContext.Quizzes.FindAsync(id);
            if (quiz == null)
                throw ThrowHelper.NotFound("Quiz", id);

            DbQuibbleUser user = await GetCurrentUserAsync();
            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised("You are not the quiz owner.");

            quiz.Title = newTitle;
            await DbContext.SaveChangesAsync();
            await QuizEvents.InvokeTitleUpdatedAsync(id, newTitle);
        }
    }
}
