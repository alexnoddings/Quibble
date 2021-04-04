using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Quibble.Server.Data;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Hubs;
using Quibble.Shared.Models;

namespace Quibble.Server.Hubs
{
    [Authorize]
    public class EventHub : Hub<IEventClient>, IEventHub
    {
        private readonly AppDbContext _dbContext;

        private string UserIdStr => Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                                    ?? throw new InvalidOperationException("User is not authenticated.");
        private Guid UserId => Guid.Parse(UserIdStr);

        public EventHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HubMethodName("Join")]
        public async Task JoinAsync(Guid quizId)
        {
            var quiz = await _dbContext.Quizzes.FindAsync(quizId);
            if (quiz is null)
                throw new HubException("Quiz not found.");

            var userId = UserId;
            if (quiz.OwnerId == userId)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizGroupName(quizId));
                await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizHostGroupName(quizId));
                return;
            }

            if (quiz.State == QuizState.Open)
            {
                if (quiz.Participants.None(participant => participant.Id == userId))
                {
                    var participant = new DbParticipant {Quiz = quiz, UserId = UserId};
                    quiz.Participants.Add(participant);
                    await _dbContext.SaveChangesAsync();
                }

                // ToDo: return quiz
                return;
            }

            throw new HubException("Quiz is not open.");
        }

        [HubMethodName("Leave")]
        public async Task LeaveAsync(Guid quizId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetQuizGroupName(quizId));
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetQuizHostGroupName(quizId));
        }

        private static string GetQuizGroupName(Guid quizId) => $"quiz::{quizId}";
        private static string GetQuizHostGroupName(Guid quizId) => $"quiz::{quizId}::host";
    }
}
