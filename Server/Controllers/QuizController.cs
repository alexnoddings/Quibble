using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data;
using Quibble.Server.Data.Models;
using Quibble.Server.Extensions;
using Quibble.Shared.Api;
using Quibble.Shared.Api.Quiz;
using Quibble.Shared.Models;

namespace Quibble.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ApiControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public QuizController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ApiResponse<Quiz>> CreateAsync(CreateQuizRequest request)
        {
            var dbQuiz = new DbQuiz
            {
                Id = Guid.Empty,
                OwnerId = AppUser.DeletedUserId,
                Title = string.IsNullOrEmpty(request.Title) ? "New Quiz" : request.Title,
                State = QuizState.InDevelopment,
                CreatedAt = DateTime.UtcNow,
            };
            _dbContext.Quizzes.Add(dbQuiz);
            await _dbContext.SaveChangesAsync();

            var quiz = _mapper.Map<Quiz>(dbQuiz);
            return SuccessResponse(quiz);
        }

        [HttpGet("{QuizId}")]
        public async Task<ApiResponse<Quiz>> GetAsync(Guid quizId)
        {
            var dbQuiz = await _dbContext.Quizzes.Include(quiz => quiz.Participants).FindAsync(quizId);
            if (dbQuiz is null)
                return ErrorResponse<Quiz>("Quiz not found.");

            var userId = UserId;
            if (dbQuiz.OwnerId != userId)
            {
                if (dbQuiz.State != QuizState.Open)
                    return ErrorResponse<Quiz>("This quiz is not open yet.");

                if (dbQuiz.Participants.None(participant => participant.UserId == userId))
                    return ErrorResponse<Quiz>("You haven't joined this quiz yet.");
            }

            var quiz = _mapper.Map<Quiz>(dbQuiz);
            return SuccessResponse(quiz);
        }

        [HttpPost("{QuizId}/Title")]
        public async Task<ApiResponse> UpdateTitleAsync(Guid quizId, UpdateQuizTitleRequest request)
        {
            var dbQuiz = await _dbContext.Quizzes.FindAsync(quizId);

            if (dbQuiz is null)
                return ErrorResponse("Quiz not found.");

            var userId = UserId;
            if (dbQuiz.OwnerId != userId)
                return ErrorResponse("You can't edit this quiz.");

            dbQuiz.Title = request.NewTitle ?? string.Empty;
            await _dbContext.SaveChangesAsync();

            return SuccessResponse();
        }

        [HttpPost("{QuizId}/Open")]
        public async Task<ApiResponse> OpenAsync(Guid quizId)
        {
            var dbQuiz = await _dbContext.Quizzes.FindAsync(quizId);

            if (dbQuiz is null)
                return ErrorResponse("Quiz not found.");

            var userId = UserId;
            if (dbQuiz.OwnerId != userId)
                return ErrorResponse("You can't edit this quiz.");

            if (dbQuiz.State == QuizState.Open)
                return ErrorResponse("Quiz is already open.");

            dbQuiz.State = QuizState.Open;
            dbQuiz.OpenedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return SuccessResponse();
        }

        [HttpDelete("{QuizId}")]
        public async Task<ApiResponse> DeleteAsync(Guid quizId)
        {
            var dbQuiz = await _dbContext.Quizzes.FindAsync(quizId);

            if (dbQuiz is null)
                return ErrorResponse("Quiz not found.");

            var userId = UserId;
            if (dbQuiz.OwnerId != userId)
                return ErrorResponse("You can't edit this quiz.");

            if (dbQuiz.State == QuizState.Open)
                return ErrorResponse("You can't delete an open quiz.");

            _dbContext.Quizzes.Remove(dbQuiz);
            await _dbContext.SaveChangesAsync();
            return SuccessResponse();
        }
    }
}
