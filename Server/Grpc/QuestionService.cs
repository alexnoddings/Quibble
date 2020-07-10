using System;
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
using Quibble.Server.Models.Questions;
using Quibble.Server.Models.Quizzes;
using Quibble.Server.Models.Rounds;

namespace Quibble.Server.Grpc
{
    /// <summary>
    /// Implements quiz question operations.
    /// </summary>
    [Authorize]
    public class QuestionService : Common.Protos.QuestionService.QuestionServiceBase
    {
        private ApplicationDbContext DbContext { get; }
        private IHubContext<QuizHub, IQuizHubClient> QuizHubContext { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="RoundService"/>.
        /// </summary>
        /// <param name="dbContext">A <see cref="ApplicationDbContext"/>.</param>
        /// <param name="quizHubContext">A <see cref="IHubContext{QuizHub, IQuizHubClient}"/>.</param>
        public QuestionService(ApplicationDbContext dbContext, IHubContext<QuizHub, IQuizHubClient> quizHubContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            QuizHubContext = quizHubContext ?? throw new ArgumentNullException(nameof(quizHubContext));
        }

        public override async Task<QuestionInfo> Create(CreateQuestionRequest request, ServerCallContext context)
        {
            string roundIdStr = request.RoundId;
            if (string.IsNullOrWhiteSpace(roundIdStr))
                return GrpcReplyHelper.InvalidArgument<QuestionInfo>(context, $"{nameof(request.RoundId)} cannot be empty");

            if (!Guid.TryParse(roundIdStr, out Guid roundId))
                return GrpcReplyHelper.InvalidArgument<QuestionInfo>(context, $"{nameof(request.RoundId)} was not valid");

            string userId = context.GetHttpContext().User.GetUserId();
            Round parentRound = await DbContext.Rounds.FindAsync(roundId).ConfigureAwait(false);

            if (parentRound == null)
                return GrpcReplyHelper.NotFound<QuestionInfo>(context, "No round found for given Id");

            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(parentRound.QuizId).ConfigureAwait(false);
            if (parentQuiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied<QuestionInfo>(context, "You do not own this quiz");

            var newQuestion = new Question { RoundId = roundId };
            DbContext.Questions.Add(newQuestion);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            var questionInfo = ToQuestionInfo(newQuestion);
            await QuizHubContext.Clients.Group(QuizHub.GetQuizGroupName(parentQuiz.Id))
                .OnQuestionCreated(questionInfo)
                .ConfigureAwait(false);

            return questionInfo;
        }

        public override async Task<QuestionInfo> GetInfo(EntityRequest request, ServerCallContext context)
        {
            string questionIdStr = request.Id;
            if (string.IsNullOrWhiteSpace(questionIdStr))
                return GrpcReplyHelper.InvalidArgument<QuestionInfo>(context, $"{nameof(request.Id)} cannot be empty");

            if (!Guid.TryParse(questionIdStr, out Guid questionId))
                return GrpcReplyHelper.InvalidArgument<QuestionInfo>(context, $"{nameof(request.Id)} was not valid");

            Question question = await DbContext.Questions.FindAsync(questionId).ConfigureAwait(false);
            if (question == null)
                return GrpcReplyHelper.NotFound<QuestionInfo>(context, "No question found for given Id");

            string userId = context.GetHttpContext().User.GetUserId();
            Round parentRound = await DbContext.Rounds.FindAsync(question.RoundId).ConfigureAwait(false);
            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(parentRound.QuizId).ConfigureAwait(false);
            if (parentQuiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied<QuestionInfo>(context, "You do not own this quiz");

            return ToQuestionInfo(question);
        }

        public override async Task<EmptyMessage> Update(UpdateQuestionRequest request, ServerCallContext context)
        {
            string questionIdStr = request.Id;
            if (string.IsNullOrWhiteSpace(questionIdStr))
                return GrpcReplyHelper.InvalidArgument(context, $"{nameof(request.Id)} cannot be empty");

            if (!Guid.TryParse(questionIdStr, out Guid questionId))
                return GrpcReplyHelper.InvalidArgument(context, $"{nameof(request.Id)} was not valid");

            Question question = await DbContext.Questions.FindAsync(questionId).ConfigureAwait(false);
            if (question == null)
                return GrpcReplyHelper.NotFound(context, "No question found for given Id");

            string userId = context.GetHttpContext().User.GetUserId();
            Round parentRound = await DbContext.Rounds.FindAsync(question.RoundId).ConfigureAwait(false);
            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(parentRound.QuizId).ConfigureAwait(false);
            if (parentQuiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied(context, "You do not own this quiz");

            question.Body = request.NewBody;
            question.Answer = request.NewAnswer;
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await QuizHubContext.Clients.Group(QuizHub.GetQuizGroupName(parentQuiz.Id))
                .OnQuestionUpdated(ToQuestionInfo(question))
                .ConfigureAwait(false);

            return GrpcReplyHelper.EmptyMessage;
        }

        public override async Task<EmptyMessage> Delete(EntityRequest request, ServerCallContext context)
        {
            string idStr = request.Id;
            if (string.IsNullOrWhiteSpace(idStr))
                return GrpcReplyHelper.InvalidArgument(context, $"{nameof(request.Id)} cannot be empty");

            if (!Guid.TryParse(idStr, out Guid id))
                return GrpcReplyHelper.InvalidArgument(context, $"{nameof(request.Id)} was not valid");

            Question question = await DbContext.Questions.FindAsync(id).ConfigureAwait(false);
            if (question == null)
                return GrpcReplyHelper.NotFound(context, "No question found for given Id");

            string userId = context.GetHttpContext().User.GetUserId();
            Round parentRound = await DbContext.Rounds.FindAsync(question.RoundId).ConfigureAwait(false);
            Quiz parentQuiz = await DbContext.Quizzes.FindAsync(parentRound.QuizId).ConfigureAwait(false);
            if (parentQuiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied(context, "You do not own this quiz");

            string roundIdStr = question.RoundId.ToString();
            string questionIdStr = question.Id.ToString();
            DbContext.Questions.Remove(question);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await QuizHubContext.Clients.Group(QuizHub.GetQuizGroupName(parentQuiz))
                .OnQuestionDeleted(roundIdStr, questionIdStr)
                .ConfigureAwait(false);

            return GrpcReplyHelper.EmptyMessage;
        }

        /// <summary>
        /// Converts a <see cref="Question"/> to a <see cref="QuestionInfo"/>.
        /// </summary>
        /// <param name="question">The <see cref="Question"/> to convert.</param>
        /// <returns>The converted <see cref="QuestionInfo"/>.</returns>
        internal static QuestionInfo ToQuestionInfo(Question question) =>
            new QuestionInfo
            {
                Id = question.Id.ToString(),
                RoundId = question.RoundId.ToString(),
                Body = question.Body,
                Answer = question.Answer,
                State = question.State.ToProtoEnum()
            };
    }
}
