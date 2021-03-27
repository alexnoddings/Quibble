using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Quibble.Server.Data;
using Quibble.Shared.Hubs;
using Quibble.Shared.Models;

namespace Quibble.Server.Hubs
{
    [Authorize]
    public class EventHub : Hub<IEventClient>, IEventHub
    {
        private readonly AppDbContext _dbContext;

        public EventHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HubMethodName("Join")]
        public async Task JoinAsync(Guid quizId)
        {
            if (!Guid.TryParse(Context.UserIdentifier, out Guid userId))
                // not auth
                return;

            var quiz = await _dbContext.Quizzes.FindAsync(quizId);
            if (quiz is null)
                // not found
                return;

            if (quiz.OwnerId == userId)
                // owner -> return
                return;

            if (quiz.State == QuizState.Open)
            {
                if (quiz.Participants.Any(participant => participant.Id == userId))
                    // in -> return
                    return;

                // not in, add -> return
                return;
            }

            // not found (not open)
            return;
        }

        [HubMethodName("Leave")]
        public async Task LeaveAsync(Guid quizId)
        {
            await Groups.RemoveFromGroupAsync(this.Context.ConnectionId, quizId.ToString());
        }
    }
}
