using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Quibble.Common.Questions;
using Quibble.Common.Quizzes;
using Quibble.Common.Rounds;
using Quibble.Server.Data;
using Quibble.Server.Extensions.Identity;

namespace Quibble.Server.Hubs
{
    [Authorize]
    public class QuestionHub : Hub<IQuestionHubClient>, IQuestionHub
    {
        private ApplicationDbContext DbContext { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="QuestionHub"/>.
        /// </summary>
        /// <param name="dbContext">A <see cref="ApplicationDbContext"/>.</param>
        public QuestionHub(ApplicationDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private ClaimsPrincipal User => Context.GetHttpContext().User;

        public async Task<Question> CreateAsync(Question question)
        {
            HubHelpers.ThrowIfNull(question, nameof(question));

            var parentRound = await DbContext.Rounds.FindAsync(question.RoundId).ConfigureAwait(false);
            if (parentRound == null) throw new HubException("No parent round found for id");

            var parentQuiz = await DbContext.Quizzes.FindAsync(parentRound.QuizId).ConfigureAwait(false);
            string userId = User.GetUserId();
            if (parentQuiz.OwnerId != userId)
                throw new HubException("You do not own this quiz");

            question.Id = Guid.Empty;
            question.Body ??= string.Empty;
            question.Answer ??= string.Empty;
            question.State = QuestionState.Hidden;

            DbContext.Questions.Add(question);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await Clients
                .GroupExcept(HubHelpers.QuizGroupNameForQuiz(parentQuiz), Context.ConnectionId)
                .OnQuestionCreated(question)
                .ConfigureAwait(false);

            return question;
        }

        public async Task<Question> GetAsync(Guid id)
        {
            var question = await DbContext.Questions.FindAsync(id).ConfigureAwait(false);
            if (question == null) throw new HubException("No question found for id");

            Round parentRound = await DbContext.Rounds.FindAsync(question.RoundId).ConfigureAwait(false);
            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(parentRound.QuizId).ConfigureAwait(false);
            if (parentQuiz.State == QuizState.WorkInProgress)
            {
                string userId = User.GetUserId();
                if (parentQuiz.OwnerId == userId)
                {
                    return question;
                }
                throw new HubException("You do not own this quiz");
            }

            // ToDo: checks
            return question;
        }

        public async Task<Question> UpdateAsync(Question question)
        {
            HubHelpers.ThrowIfNull(question, nameof(question));

            Question foundQuestion = await DbContext.Questions.FindAsync(question.Id).ConfigureAwait(false);
            if (foundQuestion == null) throw new HubException("No question found for id");

            Round parentRound = await DbContext.Rounds.FindAsync(foundQuestion.RoundId).ConfigureAwait(false);
            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(parentRound.QuizId).ConfigureAwait(false);
            string userId = User.GetUserId();
            if (parentQuiz.OwnerId != userId) throw new HubException("You do not own this quiz");

            foundQuestion.Body = question.Body;
            foundQuestion.Answer = question.Answer;
            foundQuestion.State = question.State;

            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await Clients
                .GroupExcept(HubHelpers.QuizGroupNameForQuiz(parentQuiz), Context.ConnectionId)
                .OnQuestionUpdated(foundQuestion)
                .ConfigureAwait(false);

            return foundQuestion;
        }

        public async Task DeleteAsync(Guid id)
        {
            Question foundQuestion = await DbContext.Questions.FindAsync(id).ConfigureAwait(false);
            if (foundQuestion == null) throw new HubException("No question found for id");

            Round parentRound = await DbContext.Rounds.FindAsync(foundQuestion.RoundId).ConfigureAwait(false);
            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(parentRound.QuizId).ConfigureAwait(false);
            string userId = User.GetUserId();
            if (parentQuiz.OwnerId != userId) throw new HubException("You do not own this quiz");

            DbContext.Questions.Remove(foundQuestion);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await Clients
                .GroupExcept(HubHelpers.QuizGroupNameForQuiz(parentQuiz), Context.ConnectionId)
                .OnQuestionDeleted(id)
                .ConfigureAwait(false);
        }

        public async Task RegisterForUpdatesAsync(Guid quizId)
        {
            Quiz foundQuiz = await DbContext.Quizzes.FindAsync(quizId).ConfigureAwait(false);
            if (foundQuiz == null) throw new HubException();


            if (foundQuiz.State == QuizState.WorkInProgress)
            {
                string userId = User.GetUserId();
                if (foundQuiz.OwnerId != userId)
                {
                    throw new HubException("You do not own this quiz");
                }
            }

            await Groups
                .AddToGroupAsync(Context.ConnectionId, HubHelpers.QuizGroupNameForQuiz(foundQuiz))
                .ConfigureAwait(false);
        }
    }
}
