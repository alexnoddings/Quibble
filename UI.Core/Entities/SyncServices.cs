using Quibble.UI.Core.Events;
using Quibble.UI.Core.Services;

namespace Quibble.UI.Core.Entities
{
    internal class SyncServices
    {
        public IQuizService QuizService { get; }
        public IRoundService RoundService { get; }
        public IQuestionService QuestionService { get; }

        public IQuizEvents QuizEvents { get; }
        public IRoundEvents RoundEvents { get; }
        public IQuestionEvents QuestionEvents { get; }

        public SyncServices(IQuizService quizService, IRoundService roundService, IQuestionService questionService, IQuizEvents quizEvents, IRoundEvents roundEvents, IQuestionEvents questionEvents)
        {
            QuizService = quizService;
            RoundService = roundService;
            QuestionService = questionService;
            QuizEvents = quizEvents;
            RoundEvents = roundEvents;
            QuestionEvents = questionEvents;
        }
    }
}
