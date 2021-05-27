using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    }
}
