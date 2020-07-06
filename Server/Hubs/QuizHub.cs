using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Quibble.Common.SignalR;
using Quibble.Server.Data;
using Quibble.Server.Models.Quizzes;

namespace Quibble.Server.Hubs
{
    /// <summary>
    /// SignalR hub for <see cref="Quiz"/> related events.
    /// </summary>
    /// <see cref="IQuizHubClient"/>
    [Authorize]
    public class QuizHub : Hub<IQuizHubClient>, IInvokableQuizHub
    {
        private ApplicationDbContext DbContext { get; }


        /// <summary>
        /// Initialises a new instance of <see cref="QuizHub"/>.
        /// </summary>
        /// <param name="dbContext"></param>
        public QuizHub(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task RegisterToQuizUpdatesAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (!Guid.TryParse(id, out Guid quizId))
            {
                var inner = new ArgumentException("Invalid quiz id", nameof(id));
                throw new HubException("Invalid quiz id", inner);
            }

            var quiz = await DbContext.Quizzes.FindAsync(quizId).ConfigureAwait(false);
            if (quiz == null)
                throw new HubException("No quiz found with id");

            var userId = Context.UserIdentifier;
            if (quiz.OwnerId != userId)
                throw new HubException("You do not have permission to this quiz");

            await Groups.AddToGroupAsync(Context.ConnectionId, quiz.Id.ToString()).ConfigureAwait(false);
        }
    }
}
