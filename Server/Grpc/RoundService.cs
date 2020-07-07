using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Common.Protos;
using Quibble.Common.SignalR;
using Quibble.Server.Data;
using Quibble.Server.Extensions.Identity;
using Quibble.Server.Extensions.Models;
using Quibble.Server.Hubs;
using Quibble.Server.Models.Quizzes;
using Quibble.Server.Models.Rounds;
using RoundState = Quibble.Server.Models.Rounds.RoundState;

namespace Quibble.Server.Grpc
{
    /// <summary>
    /// Implements quiz round operations.
    /// </summary>
    [Authorize]
    public class RoundService : Common.Protos.RoundService.RoundServiceBase
    {
        private ApplicationDbContext DbContext { get; }
        private IHubContext<QuizHub, IQuizHubClient> QuizHubContext { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="RoundService"/>.
        /// </summary>
        /// <param name="dbContext">A <see cref="ApplicationDbContext"/>.</param>
        /// <param name="quizHubContext">A <see cref="IHubContext{QuizHub, IQuizHubClient}"/>.</param>
        public RoundService(ApplicationDbContext dbContext, IHubContext<QuizHub, IQuizHubClient> quizHubContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            QuizHubContext = quizHubContext ?? throw new ArgumentNullException(nameof(quizHubContext));
        }

        /// <summary>
        /// Creates a new <see cref="RoundInfo"/>.
        /// </summary>
        /// <param name="request">The <see cref="CreateRoundRequest"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous creation operation. The task result represents the created round's <see cref="RoundInfo"/>.</returns>
        public override async Task<RoundInfo> Create(CreateRoundRequest request, ServerCallContext context)
        {
            string quizIdStr = request.QuizId;
            if (string.IsNullOrWhiteSpace(quizIdStr))
                return GrpcReplyHelper.InvalidArgument<RoundInfo>(context, $"{nameof(request.QuizId)} cannot be empty");

            if (!Guid.TryParse(quizIdStr, out Guid quizId))
                return GrpcReplyHelper.InvalidArgument<RoundInfo>(context, $"{nameof(request.QuizId)} was not valid");

            string title = request.Title;
            if (string.IsNullOrWhiteSpace(title))
                return GrpcReplyHelper.InvalidArgument<RoundInfo>(context, $"{nameof(request.Title)} cannot be empty");

            string userId = context.GetHttpContext().User.GetUserId();
            Quiz quiz = await DbContext.Quizzes.FindAsync(quizId).ConfigureAwait(false);

            if (quiz == null)
                return GrpcReplyHelper.NotFound<RoundInfo>(context, "No quiz found for given Id");

            if (quiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied<RoundInfo>(context, "You do not own this quiz");

            var newRound = new Round {QuizId = quizId, Title = title};
            DbContext.Rounds.Add(newRound);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            return ToRoundInfo(newRound);
        }

        /// <summary>
        /// Gets a <see cref="RoundInfo"/>.
        /// </summary>
        /// <param name="request">The <see cref="GetEntityRequest"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result represents the found round's <see cref="RoundInfo"/>.</returns>
        public override async Task<RoundInfo> GetInfo(GetEntityRequest request, ServerCallContext context)
        {
            string idStr = request.Id;
            if (string.IsNullOrWhiteSpace(idStr))
                return GrpcReplyHelper.InvalidArgument<RoundInfo>(context, $"{nameof(request.Id)} cannot be empty");

            if (!Guid.TryParse(idStr, out Guid id))
                return GrpcReplyHelper.InvalidArgument<RoundInfo>(context, $"{nameof(request.Id)} was not valid");

            string userId = context.GetHttpContext().User.GetUserId();
            Round round = await DbContext.Rounds.FindAsync(id).ConfigureAwait(false);

            if (round == null)
                return GrpcReplyHelper.NotFound<RoundInfo>(context, "No round found for given Id");

            var parentQuiz = await DbContext.Quizzes.FindAsync(round.QuizId).ConfigureAwait(false);
            if (parentQuiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied<RoundInfo>(context, "You do not own this quiz");

            return ToRoundInfo(round);
        }

        /// <summary>
        /// Gets a <see cref="RoundFull"/>.
        /// </summary>
        /// <param name="request">The <see cref="GetEntityRequest"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result represents the found round's <see cref="RoundFull"/>.</returns>
        public override async Task<RoundFull> GetFull(GetEntityRequest request, ServerCallContext context)
        {
            string idStr = request.Id;
            if (string.IsNullOrWhiteSpace(idStr))
                return GrpcReplyHelper.InvalidArgument<RoundFull>(context, $"{nameof(request.Id)} cannot be empty");

            if (!Guid.TryParse(idStr, out Guid id))
                return GrpcReplyHelper.InvalidArgument<RoundFull>(context, $"{nameof(request.Id)} was not valid");

            string userId = context.GetHttpContext().User.GetUserId();
            Round round = await DbContext.Rounds.FindAsync(id).ConfigureAwait(false);

            if (round == null)
                return GrpcReplyHelper.NotFound<RoundFull>(context, "No round found for given Id");

            var parentQuiz = await DbContext.Quizzes.FindAsync(round.QuizId).ConfigureAwait(false);
            if (parentQuiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied<RoundFull>(context, "You do not own this quiz");

            var roundInfo = ToRoundInfo(round);
            var roundFull = new RoundFull { Info = roundInfo };
            await foreach (var question in DbContext.Questions.Where(q => q.RoundId == round.Id).AsAsyncEnumerable())
            {
                var questionInfo = QuestionService.ToQuestionInfo(question);
                roundFull.Questions.Add(questionInfo);
            }

            return roundFull;
        }

        /// <summary>
        /// Updates the title of a <see cref="RoundInfo"/>.
        /// </summary>
        /// <param name="request">The <see cref="UpdateRoundTitleRequest"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous update operation. The task result represents the updating of a <see cref="RoundInfo"/>.</returns>
        public override async Task<EmptyMessage> UpdateTitle(UpdateRoundTitleRequest request, ServerCallContext context)
        {
            string idStr = request.Id;
            if (string.IsNullOrWhiteSpace(idStr))
                return GrpcReplyHelper.InvalidArgument(context, $"{nameof(request.Id)} cannot be empty");

            if (!Guid.TryParse(idStr, out Guid id))
                return GrpcReplyHelper.InvalidArgument(context, $"{nameof(request.Id)} was not valid");

            string? newTitle = request.NewTitle?.Trim();
            if (newTitle == null || newTitle.Length < 3)
                return GrpcReplyHelper.InvalidArgument(context, $"{nameof(request.NewTitle)} must be at least 3 letters long excluding spaces");

            string userId = context.GetHttpContext().User.GetUserId();
            Round round = await DbContext.Rounds.FindAsync(id).ConfigureAwait(false);

            if (round == null)
                return GrpcReplyHelper.NotFound(context, "No round found for given Id");

            var parentQuiz = await DbContext.Quizzes.FindAsync(round.QuizId).ConfigureAwait(false);
            if (parentQuiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied(context, "You do not own this quiz");

            round.Title = newTitle;
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await QuizHubContext.Clients.Group(QuizHub.GetQuizGroupName(parentQuiz))
                .OnRoundUpdated(round.Title, round.State.ToProtoEnum())
                .ConfigureAwait(false);

            return GrpcReplyHelper.EmptyMessage;
        }

        /// <summary>
        /// Updates the state of a <see cref="RoundInfo"/>.
        /// </summary>
        /// <param name="request">The <see cref="UpdateRoundStateRequest"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous update operation. The task result represents the updating of a <see cref="RoundInfo"/>.</returns>
        public override async Task<EmptyMessage> UpdateState(UpdateRoundStateRequest request, ServerCallContext context)
        {
            string idStr = request.Id;
            if (string.IsNullOrWhiteSpace(idStr))
                return GrpcReplyHelper.InvalidArgument(context, $"{nameof(request.Id)} cannot be empty");

            if (!Guid.TryParse(idStr, out Guid id))
                return GrpcReplyHelper.InvalidArgument(context, $"{nameof(request.Id)} was not valid");

            RoundState newState = request.NewState.ToModelEnum();

            string userId = context.GetHttpContext().User.GetUserId();
            Round round = await DbContext.Rounds.FindAsync(id).ConfigureAwait(false);

            if (round == null)
                return GrpcReplyHelper.NotFound(context, "No round found for given Id");

            var parentQuiz = await DbContext.Quizzes.FindAsync(round.QuizId).ConfigureAwait(false);
            if (parentQuiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied(context, "You do not own this quiz");

            round.State = newState;
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await QuizHubContext.Clients.Group(QuizHub.GetQuizGroupName(parentQuiz))
                .OnRoundUpdated(round.Title, round.State.ToProtoEnum())
                .ConfigureAwait(false);

            return GrpcReplyHelper.EmptyMessage;
        }

        /// <summary>
        /// Converts a <see cref="Round"/> to a <see cref="RoundInfo"/>.
        /// </summary>
        /// <param name="round">The <see cref="Round"/> to convert.</param>
        /// <returns>The converted <see cref="RoundInfo"/>.</returns>
        internal static RoundInfo ToRoundInfo(Round round) =>
            new RoundInfo
            {
                Id = round.Id.ToString(), 
                QuizId = round.QuizId.ToString(), 
                State = round.State.ToProtoEnum(), 
                Title = round.Title
            };
    }
}
