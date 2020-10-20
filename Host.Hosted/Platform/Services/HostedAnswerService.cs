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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quibble.Host.Hosted.Platform.Services
{
    [Authorize]
    public class HostedAnswerService : IAnswerService
    {
        private IQuizRepository QuizRepository { get; }
        private IRoundRepository RoundRepository { get; }
        private IQuestionRepository QuestionRepository { get; }
        private IAnswerRepository AnswerRepository { get; }
        private IAnswerEventsInvoker AnswerEvents { get; }
        private IParticipantRepository ParticipantRepository { get; }
        private IUserContextAccessor UserContextAccessor { get; }

        public HostedAnswerService(IQuizRepository quizRepository,
            IRoundRepository roundRepository,
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository,
            IAnswerEventsInvoker answerEvents,
            IParticipantRepository participantRepository,
            IUserContextAccessor userContextAccessor)
        {
            QuizRepository = quizRepository;
            RoundRepository = roundRepository;
            QuestionRepository = questionRepository;
            AnswerRepository = answerRepository;
            AnswerEvents = answerEvents;
            ParticipantRepository = participantRepository;
            UserContextAccessor = userContextAccessor;
        }

        public async Task<List<DtoAnswer>> GetForQuizAsync(Guid quizId)
        {
            DbQuiz quiz = await QuizRepository.GetAsync(quizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.PublishedAt == null && user.Id != quiz.OwnerId)
                throw ThrowHelper.NotFound("Quiz", quizId);

            IEnumerable<DbParticipantAnswer> answers =
                user.Id == quiz.OwnerId
                ? await AnswerRepository.GetForQuizAsync(quizId)
                : await AnswerRepository.GetForQuizAndUserAsync(quizId, user.Id);

            return answers.Select(a => new DtoAnswer(a)).ToList();
        }

        public async Task UpdateMarkAsync(Guid id, AnswerMark newMark)
        {
            DbParticipantAnswer answer = await AnswerRepository.GetAsync(id);
            DbQuestion question = await QuestionRepository.GetAsync(answer.QuestionId);
            DbRound round = await RoundRepository.GetAsync(question.RoundId);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.PublishedAt == null && user.Id != quiz.OwnerId)
                throw ThrowHelper.NotFound("Quiz", quiz.Id);

            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            await AnswerRepository.UpdateAnswerMarkAsync(id, newMark);
            await AnswerEvents.InvokeAnswerMarkUpdatedAsync(id, newMark);
        }

        public async Task UpdateTextAsync(Guid id, string newText, Guid initiatorToken)
        {
            DbParticipantAnswer answer = await AnswerRepository.GetAsync(id);
            DbParticipant participant = await ParticipantRepository.GetAsync(answer.ParticipantId);
            DbQuestion question = await QuestionRepository.GetAsync(answer.QuestionId);
            DbRound round = await RoundRepository.GetAsync(question.RoundId);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.PublishedAt == null && user.Id != quiz.OwnerId)
                throw ThrowHelper.NotFound("Quiz", quiz.Id);

            if (user.Id != participant.UserId)
                throw ThrowHelper.Unauthorised("You do not own this answer");

            await AnswerRepository.UpdateAnswerTextAsync(id, newText);
            await AnswerEvents.InvokeAnswerTextUpdatedAsync(id, newText, initiatorToken);
        }
    }
}
