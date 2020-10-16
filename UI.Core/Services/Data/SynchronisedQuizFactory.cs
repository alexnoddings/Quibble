using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quibble.UI.Core.Entities;
using Quibble.UI.Core.Events;

namespace Quibble.UI.Core.Services.Data
{
    public class SynchronisedQuizFactory : ISynchronisedQuizFactory
    {
        private IQuizService QuizService { get; }
        private IQuizEvents QuizEvents { get; }

        private IRoundService RoundService { get; }
        private IRoundEvents RoundEvents { get; }

        private IQuestionService QuestionService { get; }
        private IQuestionEvents QuestionEvents { get; }

        private IParticipantService ParticipantService { get; }
        private IParticipantEvents ParticipantEvents { get; }

        private IAnswerService AnswerService { get; }
        private IAnswerEvents AnswerEvents { get; }

        public SynchronisedQuizFactory(
            IQuizService quizService, IQuizEvents quizEvents,
            IRoundService roundService, IRoundEvents roundEvents,
            IQuestionService questionService, IQuestionEvents questionEvents,
            IParticipantService participantService, IParticipantEvents participantEvents,
            IAnswerService answerService, IAnswerEvents answerEvents)
        {
            QuizService = quizService;
            QuizEvents = quizEvents;

            RoundService = roundService;
            RoundEvents = roundEvents;

            QuestionService = questionService;
            QuestionEvents = questionEvents;

            ParticipantService = participantService;
            ParticipantEvents = participantEvents;

            AnswerService = answerService;
            AnswerEvents = answerEvents;
        }

        public async Task<SyncedQuiz> GetAsync(Guid quizId)
        {
            SyncServices services = CreateSyncServices();

            DtoQuiz quiz = await QuizService.GetAsync(quizId);

            List<DtoQuestion> questions = await QuestionService.GetForQuizAsync(quizId);
            var syncedQuestions = questions.Select(q => new SyncedQuestion(q, services)).ToList();

            List<DtoRound> rounds = await RoundService.GetForQuizAsync(quizId);
            var syncedRounds = rounds.Select(r => new SyncedRound(r, syncedQuestions.Where(q => q.RoundId == r.Id), services)).ToList();

            List<DtoParticipant> participants = await ParticipantService.GetForQuizAsync(quizId);
            var syncedParticipant = participants.Select(p => new SyncedParticipant(p, services));

            return new SyncedQuiz(quiz, syncedRounds, syncedParticipant, services);
        }

        private SyncServices CreateSyncServices() =>
            new SyncServices(
                QuizService, QuizEvents,
                RoundService, RoundEvents,
                QuestionService, QuestionEvents,
                ParticipantService, ParticipantEvents,
                AnswerService, AnswerEvents
                );
    }
}
