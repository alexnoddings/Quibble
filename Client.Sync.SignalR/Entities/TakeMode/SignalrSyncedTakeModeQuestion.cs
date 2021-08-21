using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.SignalR.Entities.TakeMode
{
    internal sealed class SignalrSyncedTakeModeQuestion : SignalrSyncedEntity, ISyncedTakeModeQuestion
    {
        public Guid RoundId { get; }
        public string Text { get; }
        public string Answer { get; private set; }
        public decimal Points { get; }
        public QuestionState State { get; private set; }
        public int Order { get; }

        internal SignalrSyncedTakeModeRound SyncedRound { get; }
        public ISyncedTakeModeRound Round => SyncedRound;

        internal SignalrSyncedTakeModeSubmittedAnswer? SyncedSubmittedAnswer { get; set; }
        public ISyncedTakeModeSubmittedAnswer? SubmittedAnswer => SyncedSubmittedAnswer;

        public SignalrSyncedTakeModeQuestion(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IQuestion question, SignalrSyncedTakeModeRound round)
            : base(logger, hubConnection)
        {
            Id = question.Id;
            RoundId = question.RoundId;
            Text = question.Text;
            Answer = question.Answer;
            Points = question.Points;
            State = question.State;
            Order = question.Order;

            SyncedRound = round;

            AddFilteredEventHandler<string>(c => c.OnQuestionAnswerUpdatedAsync, HandleAnswerUpdatedAsync);
            AddFilteredEventHandler<QuestionState>(c => c.OnQuestionStateUpdatedAsync, HandleStateUpdatedAsync);
        }

        private Task HandleAnswerUpdatedAsync(string newAnswer)
        {
            Answer = newAnswer;
            return OnUpdatedAsync();
        }

        private Task HandleStateUpdatedAsync(QuestionState newState)
        {
            State = newState;
            return OnUpdatedAsync();
        }

        public override int GetStateStamp() =>
            GenerateStateStamp(Answer, State, SyncedSubmittedAnswer);
    }
}
