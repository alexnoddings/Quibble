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
    [Authorize]
    public class QuizService : Common.Protos.QuizService.QuizServiceBase
    {
        private ApplicationDbContext DbContext { get; }
        private IHubContext<QuizHub, IQuizHub> QuizHubContext { get; }

        public QuizService(ApplicationDbContext dbContext, IHubContext<QuizHub, IQuizHub> quizHubContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            QuizHubContext = quizHubContext ?? throw new ArgumentNullException(nameof(quizHubContext));
        }

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

        public override async Task<QuizInfo> Get(GetQuizRequest request, ServerCallContext context)
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

        public override async Task<GetOwnedQuizzesReply> GetOwned(EmptyMessage request, ServerCallContext context)
        {
            string userId = context.GetHttpContext().User.GetUserId();
            var quizzes = await DbContext.Quizzes.Where(q => q.OwnerId == userId).ToListAsync().ConfigureAwait(false);

            var reply = new GetOwnedQuizzesReply();
            reply.QuizInfos.AddRange(quizzes.Select(ToQuizInfo));
            return reply;
        }

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

            await QuizHubContext.Clients.All//.Group(quiz.Id.ToString())
                .OnQuizTitleUpdated(newTitle)
                .ConfigureAwait(false);

            return GrpcReplyHelper.EmptyMessage;
        }

        private static QuizInfo ToQuizInfo(Quiz quiz) =>
            new QuizInfo
            {
                Id = quiz.Id.ToString(), 
                OwnerId = quiz.OwnerId, 
                State = quiz.State.ToProtoEnum(), 
                Title = quiz.Title
            };
    }
}
