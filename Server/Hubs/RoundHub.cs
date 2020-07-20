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
    public class RoundHub : Hub<IRoundHubClient>, IRoundHub
    {
        private ApplicationDbContext DbContext { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="RoundHub"/>.
        /// </summary>
        /// <param name="dbContext">A <see cref="ApplicationDbContext"/>.</param>
        public RoundHub(ApplicationDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private ClaimsPrincipal User => Context.GetHttpContext().User;

        public async Task<Round> CreateAsync(Round round)
        {
            HubHelpers.ThrowIfNull(round, nameof(round));

            var parentQuiz = await DbContext.Quizzes.FindAsync(round.QuizId).ConfigureAwait(false);
            if (parentQuiz == null) throw new HubException("No parent quiz found for id");

            string userId = User.GetUserId();
            if (parentQuiz.OwnerId != userId)
                throw new HubException("You do not own this quiz");

            round.Id = Guid.Empty;
            round.Title ??= string.Empty;
            round.State = RoundState.Hidden;

            DbContext.Rounds.Add(round);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await Clients
                .GroupExcept(HubHelpers.QuizGroupNameForQuiz(parentQuiz), Context.ConnectionId)
                .OnRoundCreated(round)
                .ConfigureAwait(false);

            return round;
        }

        public async Task<Round> GetAsync(Guid id)
        {
            var round = await DbContext.Rounds.FindAsync(id).ConfigureAwait(false);
            if (round == null) throw new HubException("No round found for id");

            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(round.QuizId).ConfigureAwait(false);
            if (parentQuiz.State == QuizState.WorkInProgress)
            {
                string userId = User.GetUserId();
                if (parentQuiz.OwnerId == userId)
                {
                    return round;
                }
                throw new HubException("You do not own this quiz");
            }

            // ToDo: checks
            return round;
        }

        public async Task<RoundFull> GetFullAsync(Guid id)
        {
            var round = await GetAsync(id).ConfigureAwait(false);

            var roundFull = new RoundFull(round);
            var questions = await DbContext.Questions.Where(q => q.RoundId == round.Id).ToListAsync().ConfigureAwait(false);
            foreach (var question in questions)
            {
                roundFull.Questions.Add(question);
            }

            return roundFull;
        }

        public async Task<Round> UpdateAsync(Round round)
        {
            HubHelpers.ThrowIfNull(round, nameof(round));

            Round foundRound = await DbContext.Rounds.FindAsync(round.Id).ConfigureAwait(false);
            if (foundRound == null) throw new HubException("No round found for id");

            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(foundRound.QuizId).ConfigureAwait(false);
            string userId = User.GetUserId();
            if (parentQuiz.OwnerId != userId) throw new HubException("You do not own this quiz");

            foundRound.Title = round.Title;
            foundRound.State = round.State;

            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await Clients
                .GroupExcept(HubHelpers.QuizGroupNameForQuiz(parentQuiz), Context.ConnectionId)
                .OnRoundUpdated(foundRound)
                .ConfigureAwait(false);

            return foundRound;
        }

        public async Task DeleteAsync(Guid id)
        {
            Round foundRound = await DbContext.Rounds.FindAsync(id).ConfigureAwait(false);
            if (foundRound == null) throw new HubException("No round found for id");

            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(foundRound.QuizId).ConfigureAwait(false);
            string userId = User.GetUserId();
            if (parentQuiz.OwnerId != userId) throw new HubException("You do not own this quiz");

            DbContext.Rounds.Remove(foundRound);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await Clients
                .GroupExcept(HubHelpers.QuizGroupNameForQuiz(parentQuiz), Context.ConnectionId)
                .OnRoundDeleted(id)
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
