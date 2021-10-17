using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Api;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;
using Quibble.Shared.Sync.SignalR;

namespace Quibble.Server.Hub
{
    [Authorize]
    public partial class QuibbleHub : Hub<ISignalrEvents>
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

            var dbQuiz = await DbContext.Quizzes
                .Include(quiz => quiz.Participants)
                .FindAsync(quizId);

            if (dbQuiz is null)
            {
                Context.Abort();
                return;
            }

            if (dbQuiz.OwnerId == userId)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizHostGroupName(dbQuiz.Id));
                await DbContext.SaveChangesAsync();
            }
            else
            {
                if (dbQuiz.State != QuizState.Open)
                {
                    Context.Abort();
                    return;
                }

                DbParticipant? dbParticipant = dbQuiz.Participants.FirstOrDefault(participant => participant.UserId == userId);
                if (dbParticipant is null)
                {
                    var dbUser = (await DbContext.Users.FindAsync(userId))!;
                    dbParticipant = new DbParticipant { Quiz = dbQuiz, User = dbUser };
                    dbQuiz.Participants.Add(dbParticipant);
                    await DbContext.SaveChangesAsync();

                    // Only add answers for questions which haven't been locked yet
                    var dbHiddenAndUnlockedQuestionIdsQueryable =
                        from question in DbContext.Questions
                        join round in DbContext.Rounds
                            on question.RoundId equals round.Id
                        where round.QuizId == dbQuiz.Id
                              && question.State < QuestionState.Locked
                        select question.Id;
                    var dbSubmittedAnswersQueryable =
                        from questionId in dbHiddenAndUnlockedQuestionIdsQueryable
                        select new DbSubmittedAnswer { QuestionId = questionId, Participant = dbParticipant, AssignedPoints = -1, Text = string.Empty };

                    var dbSubmittedAnswers = await dbSubmittedAnswersQueryable.ToListAsync();
                    DbContext.SubmittedAnswers.AddRange(dbSubmittedAnswers);
                    await DbContext.SaveChangesAsync();

                    var participantDto = Mapper.Map<ParticipantDto>(dbParticipant);
                    var submittedAnswerDtos = Mapper.Map<List<SubmittedAnswerDto>>(dbSubmittedAnswers);

                    await QuizHostGroup(quizId).OnParticipantJoinedAsync(participantDto, submittedAnswerDtos);
                    await AllQuizParticipantsGroup(quizId).OnParticipantJoinedAsync(participantDto, new List<SubmittedAnswerDto>());
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizParticipantGroupName(dbQuiz.Id, dbParticipant.Id));
                await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizParticipantsGroupName(dbQuiz.Id));
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GetQuizGroupName(dbQuiz.Id));

            await base.OnConnectedAsync();
        }

        protected record HubExecutionContext(Guid UserId, Guid QuizId, ApiError? Error = null);
        private HubExecutionContext BuildExecutionContext()
        {
            if (GetUserId() is not { } userId)
                return new HubExecutionContext(Guid.Empty, Guid.Empty, ApiErrors.Unauthorised);

            if (GetQuizId() is not { } quizId)
                return new HubExecutionContext(Guid.Empty, Guid.Empty, ApiErrors.QuizNotFound);

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

        private static ApiResponse Success() => ApiResponse.FromSuccess();
        private static ApiResponse<TValue> Success<TValue>(TValue value) => ApiResponse.FromSuccess(value);
        private static ApiResponse Failure(ApiError error) => ApiResponse.FromError(error);
        private static ApiResponse<TValue> Failure<TValue>(ApiError error) => ApiResponse.FromError<TValue>(error);

        private ISignalrEvents AllQuizUsersGroup(Guid quizId) => Clients.Group(GetQuizGroupName(quizId));
        private ISignalrEvents AllQuizParticipantsGroup(Guid quizId) => Clients.Group(GetQuizParticipantsGroupName(quizId));
        private ISignalrEvents QuizHostGroup(Guid quizId) => Clients.Group(GetQuizHostGroupName(quizId));
        private ISignalrEvents QuizParticipantGroup(Guid quizId, Guid participantId) => Clients.Group(GetQuizParticipantGroupName(quizId, participantId));

        public static string GetQuizGroupName(Guid quizId) => $"quiz::{quizId}";
        public static string GetQuizHostGroupName(Guid quizId) => $"quiz::{quizId}::hosts";
        public static string GetQuizParticipantsGroupName(Guid quizId) => $"quiz::{quizId}::participants";
        public static string GetQuizParticipantGroupName(Guid quizId, Guid participantId) => $"quiz::{quizId}::participant::{participantId}";
    }
}
