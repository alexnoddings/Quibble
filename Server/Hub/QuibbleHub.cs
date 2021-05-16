using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Resources;

namespace Quibble.Server.Hub
{
    [Authorize]
    public partial class QuibbleHub : Hub<IQuibbleHubClient>
    {
        private AppDbContext DbContext { get; }
        private IMapper Mapper { get; }

        protected HubExecutionContext ExecutionContext => _executionContext.Value;
        private readonly Lazy<HubExecutionContext> _executionContext;

        public QuibbleHub(AppDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
            _executionContext = new Lazy<HubExecutionContext>(BuildExecutionContext);
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
                    await Clients.Group(GetQuizGroupName(quizId)).OnParticipantJoinedAsync(participant.Id, user!.UserName);
                }
            }

            await base.OnConnectedAsync();

            await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizGroupName(dbQuiz.Id));
            if (dbQuiz.OwnerId == userId)
                await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizHostGroupName(dbQuiz.Id));
        protected record HubExecutionContext(Guid UserId, Guid QuizId, string? ErrorCode = null);
        private HubExecutionContext BuildExecutionContext()
        {
            if (GetUserId() is not { } userId)
                return new HubExecutionContext(Guid.Empty, Guid.Empty, nameof(ErrorMessages.Unauthorised));

            if (GetQuizId() is not { } quizId)
                return new HubExecutionContext(Guid.Empty, Guid.Empty, nameof(ErrorMessages.QuizNotFound));

            return new HubExecutionContext(userId, quizId);
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

        private static HubResponse Success() => HubResponse.FromSuccess();
        private static HubResponse<TValue> Success<TValue>(TValue value) => HubResponse.FromSuccess(value);
        private static HubResponse Failure(string errorCode) => HubResponse.FromError(errorCode);
        private static HubResponse<TValue> Failure<TValue>(string errorCode) => HubResponse.FromError<TValue>(errorCode);

        private IQuibbleHubClient AllQuizUsersGroup(Guid quizId) => Clients.Group(GetQuizGroupName(quizId));
        private IQuibbleHubClient AllQuizParticipantsGroup(Guid quizId) => Clients.Group(GetQuizParticipantsGroupName(quizId));
        private IQuibbleHubClient QuizHostGroup(Guid quizId) => Clients.Group(GetQuizHostGroupName(quizId));
        private IQuibbleHubClient QuizParticipantGroup(Guid quizId, Guid participantId) => Clients.Group(GetQuizParticipantGroupName(quizId, participantId));

        public static string GetQuizGroupName(Guid quizId) => $"quiz::{quizId}";
        public static string GetQuizHostGroupName(Guid quizId) => $"quiz::{quizId}::hosts";
        public static string GetQuizParticipantsGroupName(Guid quizId) => $"quiz::{quizId}::participants";
        public static string GetQuizParticipantGroupName(Guid quizId, Guid participantId) => $"quiz::{quizId}::participant::{participantId}";
    }
}
