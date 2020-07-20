using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Common.Quizzes;
using Quibble.Common.Rounds;
using Quibble.Server.Data;
using Quibble.Server.Extensions.Identity;

namespace Quibble.Server.Hubs
{
    [Authorize]
    public class QuizHub : Hub<IQuizHubClient>, IQuizHub
    {
        private ApplicationDbContext DbContext { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="QuizHub"/>.
        /// </summary>
        /// <param name="dbContext">A <see cref="ApplicationDbContext"/>.</param>
        public QuizHub(ApplicationDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private ClaimsPrincipal User => Context.GetHttpContext().User;

        public async Task<Quiz> CreateAsync(Quiz quiz)
        {
            HubHelpers.ThrowIfNull(quiz, nameof(quiz));

            string userId = User.GetUserId();

            quiz.Id = Guid.Empty;
            quiz.OwnerId = userId;
            quiz.Title ??= string.Empty;
            quiz.State = QuizState.WorkInProgress;

            DbContext.Quizzes.Add(quiz);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            return quiz;
        }

        public async Task<Quiz> GetAsync(Guid id)
        {
            Quiz quiz = await DbContext.Quizzes.FindAsync(id).ConfigureAwait(false);
            if (quiz == null) throw new HubException("No quiz found for id");

            if (quiz.State == QuizState.WorkInProgress)
            {
                string userId = User.GetUserId();
                if (quiz.OwnerId == userId)
                {
                    return quiz;
                }
                throw new HubException("You do not own this quiz");
            }

            return quiz;
        }

        public async Task<QuizFull> GetFullAsync(Guid id)
        {
            var quiz = await GetAsync(id).ConfigureAwait(false);

            var quizFull = new QuizFull(quiz);
            var rounds = await DbContext.Rounds.Where(r => r.QuizId == quiz.Id).ToListAsync().ConfigureAwait(false);
            foreach (var round in rounds)
            {
                var roundFull = new RoundFull(round);
                var questions = await DbContext.Questions.Where(q => q.RoundId == roundFull.Id).ToListAsync().ConfigureAwait(false);
                foreach (var question in questions)
                {
                    roundFull.Questions.Add(question);
                }
                quizFull.Rounds.Add(roundFull);
            }

            return quizFull;
        }

        public async Task<Quiz> UpdateAsync(Quiz quiz)
        {
            HubHelpers.ThrowIfNull(quiz, nameof(quiz));

            Quiz foundQuiz = await DbContext.Quizzes.FindAsync(quiz.Id).ConfigureAwait(false);
            if (foundQuiz == null) throw new HubException("No quiz found for id");

            string userId = User.GetUserId();
            if (foundQuiz.OwnerId != userId) throw new HubException("You do not own this quiz");

            foundQuiz.Title = quiz.Title;
            foundQuiz.State = quiz.State;

            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await Clients
                .GroupExcept(HubHelpers.QuizGroupNameForQuiz(foundQuiz), Context.ConnectionId)
                .OnQuizUpdated(foundQuiz)
                .ConfigureAwait(false);

            return foundQuiz;
        }

        public async Task DeleteAsync(Guid id)
        {
            Quiz foundQuiz = await DbContext.Quizzes.FindAsync(id).ConfigureAwait(false);
            if (foundQuiz == null) throw new HubException("No quiz found for id");

            string userId = User.GetUserId();
            if (foundQuiz.OwnerId != userId) throw new HubException("You do not own this quiz");

            DbContext.Quizzes.Remove(foundQuiz);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await Clients
                .GroupExcept(HubHelpers.QuizGroupNameForQuiz(foundQuiz), Context.ConnectionId)
                .OnQuizDeleted(id)
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
