using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Api;
using Quibble.Shared.Hubs;
using Quibble.Shared.Models;
using Quibble.Shared.Resources;

namespace Quibble.Server.Hubs
{
    [Authorize]
    public partial class QuibbleHub : Hub<IEventClient>, IEventHub
    {
        private AppDbContext DbContext { get; }
        private IMapper Mapper { get; }

        public QuibbleHub(AppDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            if (GetUserId() is not { } userId || GetQuizId() is not { } quizId)
            {
                Context.Abort();
                return;
            }

            var dbQuiz = await DbContext.Quizzes.Include(quiz => quiz.Participants).FindAsync(quizId);
            if (dbQuiz is null)
            {
                Context.Abort();
                return;
            }

            if (dbQuiz.OwnerId != userId)
            {
                if (dbQuiz.State != QuizState.Open)
                {
                    Context.Abort();
                    return;
                }

                if (dbQuiz.Participants.None(participant => participant.UserId == userId))
                {
                    var user = await DbContext.Users.FindAsync(userId);
                    var participant = new DbParticipant { Quiz = dbQuiz, UserId = userId };
                    dbQuiz.Participants.Add(participant);
                    await DbContext.SaveChangesAsync();
                    await Clients.Group(GetQuizGroupName(quizId)).OnParticipantJoinedAsync(participant.Id, user.UserName);
                }
            }

            await base.OnConnectedAsync();

            await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizGroupName(dbQuiz.Id));
            if (dbQuiz.OwnerId == userId)
                await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizHostGroupName(dbQuiz.Id));
        }

        private Guid? GetUserId()
        {
            if (Context.UserIdentifier is null || !Guid.TryParse(Context.UserIdentifier, out Guid userId))
                throw new HubException("Unauthorised.");
            return userId;
        }

        private Guid? GetQuizId()
        {
            var httpContext = Context.GetHttpContext();
            var quizIdObject = httpContext?.Request.RouteValues["QuizId"];
            if (quizIdObject is not string quizIdString)
                return null;
            if (!Guid.TryParse(quizIdString, out Guid quizId))
                return null;
            return quizId;
        }

        private HubExecutionContext ExecutionContext
        {
            get
            {
                if (GetUserId() is not { } userId)
                    return new HubExecutionContext(Guid.Empty, Guid.Empty, nameof(ErrorMessages.Unauthorised));

                if (GetQuizId() is not { } quizId)
                    return new HubExecutionContext(Guid.Empty, Guid.Empty, nameof(ErrorMessages.QuizNotFound));

                return new HubExecutionContext(userId, quizId);
            }
        }

        private record HubExecutionContext(Guid UserId, Guid QuizId, string? ErrorCode = null);

        private HubResponse Success() => new(true);
        private HubResponse<TValue> Success<TValue>(TValue? value = default) => new(true, null, value);
        private HubResponse Failure(string? errorCode = null) => new(false, errorCode);
        private HubResponse<TValue> Failure<TValue>(string? errorCode = null) => new(false, errorCode);

        private IEventClient QuizGroup(Guid quizId) => Clients.Group(GetQuizGroupName(quizId));
        private IEventClient QuizHostGroup(Guid quizId) => Clients.Group(GetQuizHostGroupName(quizId));

        public static string GetQuizGroupName(Guid quizId) => $"quiz::{quizId}";
        public static string GetQuizHostGroupName(Guid quizId) => $"quiz::{quizId}::host";
    }
}
