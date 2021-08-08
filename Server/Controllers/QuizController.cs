using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Entities;
using Quibble.Shared.Models;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class QuizController : ControllerBase
    {
        private static MapperConfiguration QuizDtoMapping { get; } =
            new(config => config.CreateMap<DbQuiz, QuizDto>());

        private AppDbContext DbContext { get; }

        public QuizController(AppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpPost]
        public async Task<Guid> CreateAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userQuizCount = await DbContext.Quizzes.Where(quiz => quiz.OwnerId == userId).CountAsync();
            var title = $"Quiz #{userQuizCount + 1}";

            var dbQuiz = new DbQuiz
            {
                Id = Guid.Empty,
                OwnerId = userId,
                Title = title,
                State = QuizState.InDevelopment,
                CreatedAt = DateTime.UtcNow,
            };
            DbContext.Quizzes.Add(dbQuiz);
            await DbContext.SaveChangesAsync();

            return dbQuiz.Id;
        }

        [HttpGet("{QuizId:guid}/negotiate")]
        public async Task<QuizNegotiationDto?> NegotiateAsync(Guid quizId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var dbQuiz = await DbContext.Quizzes.Include(quiz => quiz.Participants).FindAsync(quizId);
            if (dbQuiz is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return null;
            }

            if (dbQuiz.OwnerId != userId && dbQuiz.State == QuizState.InDevelopment)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                return new QuizNegotiationDto { CanEdit = false, State = QuizState.InDevelopment };
            }

            Response.StatusCode = StatusCodes.Status200OK;
            return new QuizNegotiationDto { CanEdit = userId == dbQuiz.OwnerId, State = dbQuiz.State };
        }

        [HttpGet]
        public async Task<UserQuizzes> GetUserQuizzesAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var inDevelopmentQuizzesQueryable =
                (from quiz in DbContext.Quizzes
                 where quiz.OwnerId == userId
                 where quiz.State == QuizState.InDevelopment
                 orderby quiz.CreatedAt descending
                 select quiz)
                .ProjectTo<QuizDto>(QuizDtoMapping);


            var participatedQuizzesQueryable =
                (from quiz in DbContext.Quizzes
                 join participant in DbContext.Participants
                     on quiz.Id equals participant.QuizId
                 where participant.UserId == userId
                 orderby quiz.OpenedAt descending
                 select quiz)
                .ProjectTo<QuizDto>(QuizDtoMapping);

            var hostedQuizzesQueryable =
                (from quiz in DbContext.Quizzes
                 where quiz.OwnerId == userId
                 where quiz.State == QuizState.Open
                 orderby quiz.OpenedAt descending
                 select quiz)
                .ProjectTo<QuizDto>(QuizDtoMapping);

            return new UserQuizzes
            {
                InDevelopmentQuizzes = await inDevelopmentQuizzesQueryable.Take(5).ToListAsync(),
                ParticipatedQuizzes = await participatedQuizzesQueryable.Take(5).ToListAsync(),
                HostedQuizzes = await hostedQuizzesQueryable.Take(5).ToListAsync(),
            };
        }

        [AllowAnonymous]
        [HttpGet("stats")]
        public async Task<SiteStats> GetSiteStatsAsync()
        {
            var openQuizzesWithParticipants =
                from quiz in DbContext.Quizzes
                where quiz.State == QuizState.Open
                where quiz.Participants.Any()
                select quiz;

            var quizCount = await openQuizzesWithParticipants.CountAsync();

            var markedSubmittedAnswers =
                from submittedAnswer in DbContext.SubmittedAnswers
                where submittedAnswer.AssignedPoints >= 0
                select submittedAnswer;

            var submittedAnswerCount = await markedSubmittedAnswers.CountAsync();

            decimal averagePercent;
            if (submittedAnswerCount == 0)
            {
                averagePercent = 0;
            }
            else
            {
                var submittedAnswersPercents =
                    from question in DbContext.Questions
                    join submittedAnswer in markedSubmittedAnswers
                        on question.Id equals submittedAnswer.QuestionId
                    select submittedAnswer.AssignedPoints / question.Points;

                averagePercent = await submittedAnswersPercents.AverageAsync();
            }

            return new SiteStats
            {
                QuizCount = quizCount,
                SubmittedAnswerCount = submittedAnswerCount,
                AveragePercent = averagePercent
            };
        }
    }
}
