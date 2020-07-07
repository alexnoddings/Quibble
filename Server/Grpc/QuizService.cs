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

namespace Quibble.Server.Grpc
{
    /// <summary>
    /// Implements quiz operations.
    /// </summary>
    [Authorize]
    public class QuizService : Common.Protos.QuizService.QuizServiceBase
    {
        private ApplicationDbContext DbContext { get; }
        private IHubContext<QuizHub, IQuizHubClient> QuizHubContext { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="QuizService"/>.
        /// </summary>
        /// <param name="dbContext">A <see cref="ApplicationDbContext"/>.</param>
        /// <param name="quizHubContext">A <see cref="IHubContext{QuizHub, IQuizHubClient}"/>.</param>
        public QuizService(ApplicationDbContext dbContext, IHubContext<QuizHub, IQuizHubClient> quizHubContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            QuizHubContext = quizHubContext ?? throw new ArgumentNullException(nameof(quizHubContext));
        }

        /// <summary>
        /// Creates a new <see cref="QuizInfo"/>.
        /// </summary>
        /// <param name="request">The <see cref="CreateQuizRequest"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous creation operation. The task result represents the created quiz's <see cref="QuizInfo"/>.</returns>
        public override async Task<QuizInfo> Create(CreateQuizRequest request, ServerCallContext context)
        {
            string title = request.Title;
            if (string.IsNullOrWhiteSpace(title))
                return GrpcReplyHelper.InvalidArgument<QuizInfo>(context, $"{nameof(request.Title)} cannot be empty");

            string userId = context.GetHttpContext().User.GetUserId();
            var newQuiz = new Quiz {Title = title, OwnerId = userId};

            DbContext.Quizzes.Add(newQuiz);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            return ToQuizInfo(newQuiz);
        }

        /// <summary>
        /// Gets a <see cref="QuizInfo"/>.
        /// </summary>
        /// <param name="request">The <see cref="GetEntityRequest"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result represents the found quiz's <see cref="QuizInfo"/>.</returns>
        public override async Task<QuizInfo> GetInfo(GetEntityRequest request, ServerCallContext context)
        {
            string idStr = request.Id;
            if (string.IsNullOrWhiteSpace(idStr))
                return GrpcReplyHelper.InvalidArgument<QuizInfo>(context, $"{nameof(request.Id)} cannot be empty");

            if (!Guid.TryParse(idStr, out Guid id))
                return GrpcReplyHelper.InvalidArgument<QuizInfo>(context, $"{nameof(request.Id)} was not valid");

            string userId = context.GetHttpContext().User.GetUserId();
            Quiz quiz = await DbContext.Quizzes.FindAsync(id).ConfigureAwait(false);

            if (quiz == null)
                return GrpcReplyHelper.NotFound<QuizInfo>(context, "No quiz found for given Id");

            if (quiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied<QuizInfo>(context, "You do not own this quiz");

            return ToQuizInfo(quiz);
        }

        /// <summary>
        /// Gets a <see cref="QuizFull"/>.
        /// </summary>
        /// <param name="request">The <see cref="GetEntityRequest"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result represents the found quiz's <see cref="QuizInfo"/>.</returns>
        public override async Task<QuizFull> GetFull(GetEntityRequest request, ServerCallContext context)
        {
            string idStr = request.Id;
            if (string.IsNullOrWhiteSpace(idStr))
                return GrpcReplyHelper.InvalidArgument<QuizFull>(context, $"{nameof(request.Id)} cannot be empty");

            if (!Guid.TryParse(idStr, out Guid id))
                return GrpcReplyHelper.InvalidArgument<QuizFull>(context, $"{nameof(request.Id)} was not valid");

            string userId = context.GetHttpContext().User.GetUserId();
            Quiz quiz = await DbContext.Quizzes.FindAsync(id).ConfigureAwait(false);

            if (quiz == null)
                return GrpcReplyHelper.NotFound<QuizFull>(context, "No quiz found for given Id");

            if (quiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied<QuizFull>(context, "You do not own this quiz");

            var quizFull = new QuizFull {Info = ToQuizInfo(quiz)};

            await foreach (var round in DbContext.Rounds.Where(r => r.QuizId == quiz.Id).AsAsyncEnumerable())
            {
                var roundInfo = RoundService.ToRoundInfo(round);
                var roundFull = new RoundFull {Info = roundInfo};
                await foreach (var question in DbContext.Questions.Where(q => q.RoundId == round.Id).AsAsyncEnumerable())
                {
                    var questionInfo = QuestionService.ToQuestionInfo(question);
                    roundFull.Questions.Add(questionInfo);
                }
                quizFull.Rounds.Add(roundFull);
            }

            return quizFull;
        }

        /// <summary>
        /// Gets the <see cref="QuizInfo"/>s owned by the calling user.
        /// </summary>
        /// <param name="request">The <see cref="EmptyMessage"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result represents the found quizzes' <see cref="QuizInfo"/>.</returns>
        public override async Task<GetOwnedInfosReply> GetOwnedInfos(EmptyMessage request, ServerCallContext context)
        {
            string userId = context.GetHttpContext().User.GetUserId();
            var quizzes = await DbContext.Quizzes.Where(q => q.OwnerId == userId).ToListAsync().ConfigureAwait(false);

            var reply = new GetOwnedInfosReply();
            reply.QuizInfos.AddRange(quizzes.Select(ToQuizInfo));
            return reply;
        }

        /// <summary>
        /// Updates the title of a <see cref="QuizInfo"/>.
        /// </summary>
        /// <param name="request">The <see cref="UpdateQuizTitleRequest"/>.</param>
        /// <param name="context">The <see cref="ServerCallContext"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous update operation. The task result represents the updating of a <see cref="QuizInfo"/>.</returns>
        public override async Task<EmptyMessage> UpdateTitle(UpdateQuizTitleRequest request, ServerCallContext context)
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
            Quiz quiz = await DbContext.Quizzes.FindAsync(id).ConfigureAwait(false);

            if (quiz == null)
                return GrpcReplyHelper.NotFound(context, "No quiz found for given Id");

            if (quiz.OwnerId != userId)
                return GrpcReplyHelper.PermissionDenied(context, "You do not own this quiz");

            quiz.Title = newTitle;
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            await QuizHubContext.Clients.Group(quiz.Id.ToString())
                .OnQuizTitleUpdated(newTitle)
                .ConfigureAwait(false);

            return GrpcReplyHelper.EmptyMessage;
        }

        /// <summary>
        /// Converts a <see cref="Quiz"/> to a <see cref="QuizInfo"/>.
        /// </summary>
        /// <param name="quiz">The <see cref="Quiz"/> to convert.</param>
        /// <returns>The converted <see cref="QuizInfo"/>.</returns>
        internal static QuizInfo ToQuizInfo(Quiz quiz) =>
            new QuizInfo
            {
                Id = quiz.Id.ToString(), 
                OwnerId = quiz.OwnerId, 
                State = quiz.State.ToProtoEnum(), 
                Title = quiz.Title
            };
    }
}
