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

            List<DtoAnswer> dtoAnswers = await AnswerService.GetForQuizAsync(quizId);
            var syncedAnswers = from answer in dtoAnswers
                                select new SyncedAnswer(answer, services);

            List<DtoQuestion> dtoQuestions = await QuestionService.GetForQuizAsync(quizId);
            var syncedQuestions = from question in dtoQuestions
                                  join answer in syncedAnswers on question.Id equals answer.QuestionId into answers
                                  select new SyncedQuestion(question, answers, services);

            List<DtoRound> dtoRounds = await RoundService.GetForQuizAsync(quizId);
            var syncedRounds = from round in dtoRounds
                               join question in syncedQuestions on round.Id equals question.RoundId into questions
                               select new SyncedRound(round, questions, services);

            List<DtoParticipant> dtoParticipants = await ParticipantService.GetForQuizAsync(quizId);
            var syncedParticipant = from participant in dtoParticipants
                                    select new SyncedParticipant(participant, services);

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
