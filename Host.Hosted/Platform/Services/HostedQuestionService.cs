using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Quibble.Core.Entities;
using Quibble.Host.Common;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;
using Quibble.Host.Common.Repositories;
using Quibble.Host.Common.Services;
using Quibble.Host.Hosted.Platform.Events;
using Quibble.UI.Core.Entities;
using Quibble.UI.Core.Services.Data;

namespace Quibble.Host.Hosted.Platform.Services
{
    [Authorize]
    public class HostedQuestionService : IQuestionService
    {
        private IQuizRepository QuizRepository { get; }
        private IParticipantRepository ParticipantRepository { get; }
        private IRoundRepository RoundRepository { get; }
        private IQuestionRepository QuestionRepository { get; }
        private IQuestionEventsInvoker QuestionEvents { get; }
        private IUserContextAccessor UserContextAccessor { get; }

        public HostedQuestionService(IQuizRepository quizRepository, IParticipantRepository participantRepository, IRoundRepository roundRepository, IQuestionRepository questionRepository, IQuestionEventsInvoker questionEvents, IUserContextAccessor userContextAccessor)
        {
            QuizRepository = quizRepository;
            ParticipantRepository = participantRepository;
            RoundRepository = roundRepository;
            QuestionRepository = questionRepository;
            QuestionEvents = questionEvents;
            UserContextAccessor = userContextAccessor;
        }

        public async Task CreateAsync(Guid parentRoundId)
        {
            DbRound round = await RoundRepository.GetAsync(parentRoundId);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            if (quiz.PublishedAt != null)
                throw ThrowHelper.InvalidOperation(ExceptionMessages.CannotEditPublishedQuiz);

            var question = new DbQuestion {RoundId = round.Id};
            await QuestionRepository.CreateAsync(question);
            await QuestionEvents.InvokeQuestionAddedAsync(question);
        }

        public async Task<List<DtoQuestion>> GetForQuizAsync(Guid quizId)
        {
            DbQuiz quiz = await QuizRepository.GetAsync(quizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.OwnerId != user.Id)
            {
                if (quiz.PublishedAt == null)
                    throw ThrowHelper.NotFound("Quiz", quizId);

                if (!await ParticipantRepository.IsParticipatingAsync(quizId, user.Id))
                    throw ThrowHelper.Unauthorised(ExceptionMessages.NotJoinedQuiz);
            }

            var questions = await QuestionRepository.GetForQuizAsync(quizId);
            return questions.Select(q => new DtoQuestion(q)).ToList();
        }

        public async Task DeleteAsync(Guid id)
        {
            DbQuestion question = await QuestionRepository.GetAsync(id);
            DbRound round = await RoundRepository.GetAsync(question.RoundId);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.OwnerId != user.Id)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            if (quiz.PublishedAt != null)
                throw ThrowHelper.InvalidOperation(ExceptionMessages.CannotEditPublishedQuiz);

            await QuestionRepository.DeleteAsync(id);
            await QuestionEvents.InvokeQuestionDeletedAsync(id);
        }

        public async Task UpdateTextAsync(Guid id, string newText)
        {
            newText ??= string.Empty;

            DbQuestion question = await QuestionRepository.GetAsync(id);
            DbRound round = await RoundRepository.GetAsync(question.RoundId);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.OwnerId != user.Id)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            if (quiz.PublishedAt != null)
                throw ThrowHelper.InvalidOperation(ExceptionMessages.CannotEditPublishedQuiz);

            await QuestionRepository.UpdateTextAsync(id, newText);
            await QuestionEvents.InvokeTextUpdatedAsync(id, newText);
        }

        public async Task UpdateCorrectAnswerAsync(Guid id, string newAnswer)
        {
            newAnswer ??= string.Empty;

            DbQuestion question = await QuestionRepository.GetAsync(id);
            DbRound round = await RoundRepository.GetAsync(question.RoundId);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.OwnerId != user.Id)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            if (quiz.PublishedAt != null)
                throw ThrowHelper.InvalidOperation(ExceptionMessages.CannotEditPublishedQuiz);

            await QuestionRepository.UpdateAnswerAsync(id, newAnswer);
            await QuestionEvents.InvokeCorrectAnswerUpdatedAsync(id, newAnswer);
        }

        public async Task UpdateStateAsync(Guid id, QuestionState newState)
        {
            DbQuestion question = await QuestionRepository.GetAsync(id);
            DbRound round = await RoundRepository.GetAsync(question.RoundId);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.OwnerId != user.Id)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            if (quiz.PublishedAt == null)
                throw ThrowHelper.InvalidOperation("Cannot update state for an unpublished quiz");

            if ((question.State == QuestionState.Hidden && newState == QuestionState.Visible)
                || (question.State == QuestionState.Visible && newState == QuestionState.Locked)
                || (question.State == QuestionState.Locked && newState == QuestionState.AnswerRevealed))
            {
                await QuestionRepository.UpdateStateAsync(id, newState);
                await QuestionEvents.InvokeStateUpdatedAsync(id, newState);
                return;
            }

            throw ThrowHelper.BadArgument(nameof(newState), "Invalid state progression.");
        }
    }
}
