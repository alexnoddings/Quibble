using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Extensions;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Server.Hub
{
    public partial class QuibbleHub
    {
        [HubMethodName(Endpoints.GetQuiz)]
        public async Task<HubResponse<FullQuizDto>> GetQuizAsync()
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure<FullQuizDto>(error);

            var dbQuiz =
                await DbContext.Quizzes
                    .Include(q => q.Participants)
                        .ThenInclude(pt => pt.User)
                    .Include(q => q.Rounds)
                        .ThenInclude(r => r.Questions)
                            .ThenInclude(qs => qs.SubmittedAnswers)
                    .FindAsync(quizId);

            if (dbQuiz is null)
                return Failure<FullQuizDto>(HubErrors.QuizNotFound);

            if (dbQuiz.OwnerId != userId && dbQuiz.State != QuizState.Open)
                return Failure<FullQuizDto>(HubErrors.QuizNotOpen);

            var quiz = Mapper.Map<QuizDto>(dbQuiz);
            List<RoundDto> rounds;
            List<ParticipantDto> participants;
            List<QuestionDto> questions;
            List<SubmittedAnswerDto> submittedAnswers;

            if (dbQuiz.State == QuizState.InDevelopment)
            {
                rounds = Mapper.Map<List<RoundDto>>(dbQuiz.Rounds);
                participants = new List<ParticipantDto>();
                questions = Mapper.Map<List<QuestionDto>>(dbQuiz.Rounds.SelectMany(r => r.Questions));
                submittedAnswers = new List<SubmittedAnswerDto>();
            }
            else if (dbQuiz.State == QuizState.Open && dbQuiz.OwnerId == userId)
            {
                rounds = Mapper.Map<List<RoundDto>>(dbQuiz.Rounds);
                participants = Mapper.Map<List<ParticipantDto>>(dbQuiz.Participants);

                var dbQuestions = dbQuiz.Rounds.SelectMany(r => r.Questions).ToList();
                questions = Mapper.Map<List<QuestionDto>>(dbQuestions);

                var dbSubmittedAnswers = dbQuestions.SelectMany(qs => qs.SubmittedAnswers).ToList();
                submittedAnswers = Mapper.Map<List<SubmittedAnswerDto>>(dbSubmittedAnswers);
            }
            else
            {
                var dbParticipant = await DbContext.Participants.FirstAsync(p => p.QuizId == quizId && p.UserId == userId);

                var visibleDbRounds = dbQuiz.Rounds.Where(r => r.State == RoundState.Open).ToList();
                rounds = Mapper.Map<List<RoundDto>>(visibleDbRounds);
                participants = Mapper.Map<List<ParticipantDto>>(dbQuiz.Participants);

                var visibleDbQuestions = visibleDbRounds.SelectMany(r => r.Questions).Where(q => q.State != QuestionState.Hidden).ToList();
                questions = Mapper.Map<List<QuestionDto>>(visibleDbQuestions);

                var userDbSubmittedAnswers = visibleDbQuestions.SelectMany(q => q.SubmittedAnswers).Where(sa => sa.ParticipantId == dbParticipant.Id).ToList();
                submittedAnswers = Mapper.Map<List<SubmittedAnswerDto>>(userDbSubmittedAnswers);
            }

            return Success(new FullQuizDto(quiz, participants, rounds, questions, submittedAnswers));
        }

        [HubMethodName(Endpoints.UpdateQuizTitle)]
        public async Task<HubResponse> UpdateQuizTitleAsync(string newTitle)
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuiz = await DbContext.Quizzes.FindAsync(quizId);
            if (dbQuiz is null)
                return Failure(HubErrors.QuizNotFound);

            if (dbQuiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbQuiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            newTitle ??= string.Empty;
            if (newTitle.Length > 100)
                return Failure(HubErrors.QuizEmpty);

            dbQuiz.Title = newTitle;
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnQuizTitleUpdatedAsync(newTitle);

            return Success();
        }

        [HubMethodName(Endpoints.OpenQuiz)]
        public async Task<HubResponse> OpenQuizAsync()
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuiz = await DbContext.Quizzes.FindAsync(quizId);
            if (dbQuiz is null)
                return Failure(HubErrors.QuizNotFound);

            if (dbQuiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbQuiz.State == QuizState.Open)
                return Failure(HubErrors.QuizAlreadyOpen);

            var quizRounds =
                from round in DbContext.Rounds
                where round.QuizId == quizId
                select round;

            var quizQuestions =
                from round in quizRounds
                join question in DbContext.Questions
                    on round.Id equals question.RoundId
                select question;

            if (!await quizQuestions.AnyAsync())
                return Failure(HubErrors.QuizEmpty);

            if (await quizRounds.AnyAsync(round => string.IsNullOrWhiteSpace(round.Title)))
                return Failure(HubErrors.RoundMissingTitle);

            if (await quizQuestions.AnyAsync(question => string.IsNullOrWhiteSpace(question.Text)))
                return Failure(HubErrors.QuestionMissingText);

            if (await quizQuestions.AnyAsync(question => string.IsNullOrWhiteSpace(question.Answer)))
                return Failure(HubErrors.QuestionMissingAnswer);

            dbQuiz.State = QuizState.Open;
            dbQuiz.OpenedAt = DateTime.UtcNow;
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnQuizOpenedAsync();

            return Success();
        }

        [HubMethodName(Endpoints.DeleteQuiz)]
        public async Task<HubResponse> DeleteQuizAsync()
        {
            (Guid userId, Guid quizId, ApiError? error) = ExecutionContext;
            if (error is not null)
                return Failure(error);

            var dbQuiz = await DbContext.Quizzes.FindAsync(quizId);
            if (dbQuiz is null)
                return Failure(HubErrors.QuizNotFound);

            if (dbQuiz.OwnerId != userId)
                return Failure(HubErrors.CantEditAsNotOwner);

            if (dbQuiz.State != QuizState.InDevelopment)
                return Failure(HubErrors.CantDeleteAsNotInDevelopment);

            DbContext.Quizzes.Remove(dbQuiz);
            await DbContext.SaveChangesAsync();

            await AllQuizUsersGroup(quizId).OnQuizDeletedAsync();

            return Success();
        }
    }
}
