using Quibble.UI.Core.Events;
using Quibble.UI.Core.Services;

namespace Quibble.UI.Core.Entities
{
    internal class SyncServices
    {
        public IQuizService QuizService { get; }
        public IRoundService RoundService { get; }
        public IQuestionService QuestionService { get; }
        public IParticipantService ParticipantService { get; }

        public IQuizEvents QuizEvents { get; }
        public IRoundEvents RoundEvents { get; }
        public IQuestionEvents QuestionEvents { get; }
        public IParticipantEvents ParticipantEvents { get; }

        public SyncServices(
            IQuizService quizService, IRoundService roundService, IQuestionService questionService, IParticipantService participantService,
            IQuizEvents quizEvents, IRoundEvents roundEvents, IQuestionEvents questionEvents, IParticipantEvents participantEvents)
        {
            QuizService = quizService;
            RoundService = roundService;
            QuestionService = questionService;
            ParticipantService = participantService;
            QuizEvents = quizEvents;
            RoundEvents = roundEvents;
            QuestionEvents = questionEvents;
            ParticipantEvents = participantEvents;
        }
    }
}
