using Quibble.UI.Core.Events;
using Quibble.UI.Core.Services.Data;

namespace Quibble.UI.Core.Entities
{
    internal class SyncServices
    {
        public IQuizService QuizService { get; }
        public IQuizEvents QuizEvents { get; }

        public IRoundService RoundService { get; }
        public IRoundEvents RoundEvents { get; }

        public IQuestionService QuestionService { get; }
        public IQuestionEvents QuestionEvents { get; }

        public IParticipantService ParticipantService { get; }
        public IParticipantEvents ParticipantEvents { get; }

        public IAnswerService AnswerService { get; }
        public IAnswerEvents AnswerEvents { get; }

        public SyncServices(
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
    }
}
