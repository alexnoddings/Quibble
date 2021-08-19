using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Internal.TakeMode
{
    internal sealed class SynchronisedTakeModeRound : SignalrSynchronisedEntity, ISynchronisedTakeModeRound
    {
        public Guid QuizId { get; }
        public string Title { get; }
        public RoundState State { get; }
        public int Order { get; }

        public event Func<ISynchronisedTakeModeQuestion, Task>? QuestionAdded;

        internal SynchronisedTakeModeQuiz SyncedQuiz { get; }
        public ISynchronisedTakeModeQuiz Quiz => SyncedQuiz;

        internal List<SynchronisedTakeModeQuestion> SyncedQuestions { get; } = new();
        public IReadOnlyList<ISynchronisedTakeModeQuestion> Questions => SyncedQuestions.AsReadOnly();

        public SynchronisedTakeModeRound(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IRound round, SynchronisedTakeModeQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = round.Id;
            QuizId = round.QuizId;
            Title = round.Title;
            State = round.State;
            Order = round.Order;

            SyncedQuiz = quiz;

            AddEventHandler<QuestionDto, SubmittedAnswerDto>(c => c.OnQuestionRevealedAsync, HandleQuestionRevealedAsync);
        }

        private async Task HandleQuestionRevealedAsync(QuestionDto question, SubmittedAnswerDto submittedAnswer)
        {
            if (question.RoundId != Id)
                return;

            var synchronisedQuestion = new SynchronisedTakeModeQuestion(Logger, HubConnection, question, this);
            var synchronisedAnswer = new SynchronisedTakeModeSubmittedAnswer(Logger, HubConnection, submittedAnswer, synchronisedQuestion);
            synchronisedQuestion.SyncedSubmittedAnswer = synchronisedAnswer;

            SyncedQuestions.Add(synchronisedQuestion);
            
            if (QuestionAdded is not null)
                await QuestionAdded.Invoke(synchronisedQuestion);

            await OnUpdatedAsync();
        }

        public override int GetStateStamp() =>
            GenerateStateStamp(State, SyncedQuestions);

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                while (SyncedQuestions.Count > 0)
                {
                    var question = SyncedQuestions[0];
                    question.Dispose();
                    SyncedQuestions.RemoveAt(0);
                }
            }

            base.Dispose(disposing);
        }
    }
}
