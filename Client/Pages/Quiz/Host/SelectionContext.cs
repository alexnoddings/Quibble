using Quibble.Client.Sync.Core;
using Quibble.Client.Sync.Extensions;

namespace Quibble.Client.Pages.Quiz.Host
{
    public class SelectionContext
    {
        private ISyncedQuiz Quiz { get; }
        public ISyncedRound Round => Quiz.Rounds[RoundNumber];
        public ISyncedQuestion Question => Round.Questions[QuestionNumber];

        public int RoundNumber { get; private set; }
        public int QuestionNumber { get; private set; }

        public event Func<SelectionChangedEventArgs, Task>? OnUpdated;

        public bool IsAtFirstQuestion => RoundNumber == 0 && QuestionNumber == 0;
        public bool IsAtLastQuestion => RoundNumber == Quiz.Rounds.Count - 1 && QuestionNumber == Round.Questions.Count - 1;

        public SelectionContext(ISyncedQuiz quiz)
        {
            Quiz = quiz;
        }

        public Task UpdateSelectionAsync(int roundNumber, int questionNumber)
        {
            var previousQuestion = Question;
            RoundNumber = Math.Clamp(roundNumber, 0, Quiz.Rounds.Count - 1);
            QuestionNumber = Math.Clamp(questionNumber, 0, Round.Questions.Count - 1);

            var eventArgs = new SelectionChangedEventArgs(previousQuestion, Question);
            return OnUpdated.InvokeAsync(eventArgs);
        }

        public Task MoveToPreviousQuestionAsync()
        {
            if (RoundNumber == 0 && QuestionNumber == 0)
                return Task.CompletedTask;

            var previousQuestion = Question;

            if (QuestionNumber == 0)
            {
                RoundNumber--;
                QuestionNumber = Round.Questions.Count - 1;
            }
            else
            {
                QuestionNumber--;
            }

            var eventArgs = new SelectionChangedEventArgs(previousQuestion, Question);
            return OnUpdated.InvokeAsync(eventArgs);
        }

        public Task MoveToNextQuestionAsync()
        {
            if (RoundNumber == Quiz.Rounds.Count - 1 && QuestionNumber == Round.Questions.Count - 1)
                return Task.CompletedTask;

            var previousQuestion = Question;

            if (QuestionNumber == Round.Questions.Count - 1)
            {
                QuestionNumber = 0;
                RoundNumber++;
            }
            else
            {
                QuestionNumber++;
            }

            var eventArgs = new SelectionChangedEventArgs(previousQuestion, Question);
            return OnUpdated.InvokeAsync(eventArgs);
        }
    }
}
