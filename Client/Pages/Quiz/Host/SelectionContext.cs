using Quibble.Client.Sync.Entities.HostMode;

namespace Quibble.Client.Pages.Quiz.Host
{
    public class SelectionContext
    {
        private ISyncedHostModeQuiz Quiz { get; }
        public ISyncedHostModeRound Round => Quiz.Rounds[RoundNumber];
        public ISyncedHostModeQuestion Question => Round.Questions[QuestionNumber];

        public int RoundNumber { get; private set; }
        public int QuestionNumber { get; private set; }

        public event Func<Task>? Updated;

        public bool IsAtFirstQuestion => RoundNumber == 0 && QuestionNumber == 0;
        public bool IsAtLastQuestion => RoundNumber == Quiz.Rounds.Count - 1 && QuestionNumber == Round.Questions.Count - 1;

        public SelectionContext(ISyncedHostModeQuiz quiz)
        {
            Quiz = quiz;
        }

        public Task UpdateSelectionAsync(int roundNumber, int questionNumber)
        {
            RoundNumber = Math.Clamp(roundNumber, 0, Quiz.Rounds.Count - 1);
            QuestionNumber = Math.Clamp(questionNumber, 0, Round.Questions.Count - 1);

            return OnUpdatedAsync();
        }

        public Task MoveToPreviousQuestionAsync()
        {
            if (RoundNumber == 0 && QuestionNumber == 0)
                return Task.CompletedTask;

            if (QuestionNumber == 0)
            {
                RoundNumber--;
                QuestionNumber = Round.Questions.Count - 1;
            }
            else
            {
                QuestionNumber--;
            }

            return OnUpdatedAsync();
        }

        public Task MoveToNextQuestionAsync()
        {
            if (RoundNumber == Quiz.Rounds.Count - 1 && QuestionNumber == Round.Questions.Count - 1)
                return Task.CompletedTask;

            if (QuestionNumber == Round.Questions.Count - 1)
            {
                QuestionNumber = 0;
                RoundNumber++;
            }
            else
            {
                QuestionNumber++;
            }

            return OnUpdatedAsync();
        }

        private Task OnUpdatedAsync() => Updated?.Invoke() ?? Task.CompletedTask;
    }
}
