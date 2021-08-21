using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.SignalR.Entities.HostMode
{
    internal sealed class SignalrSyncedHostModeRound : SignalrSyncedEntity, ISyncedHostModeRound
    {
        public Guid QuizId { get; }
        public string Title { get; }
        public RoundState State { get; private set; }
        public int Order { get; }

        internal SignalrSyncedHostModeQuiz SyncedQuiz { get; }
        public ISyncedHostModeQuiz Quiz => SyncedQuiz;

        internal List<SignalrSyncedHostModeQuestion> SyncedQuestions { get; } = new();
        public IReadOnlyList<ISyncedHostModeQuestion> Questions => SyncedQuestions.AsReadOnly();

        public SignalrSyncedHostModeRound(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IRound round, SignalrSyncedHostModeQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = round.Id;
            QuizId = round.QuizId;
            Title = round.Title;
            State = round.State;
            Order = round.Order;

            SyncedQuiz = quiz;

            AddFilteredEventHandler(c => c.OnRoundOpenedAsync, HandleRoundOpenedAsync);
        }

        public Task OpenAsync()
        {
            if (State != RoundState.Hidden)
                return Task.CompletedTask;

            return HubConnection.InvokeAsync(Endpoints.OpenRound, Id);
        }

        private Task HandleRoundOpenedAsync()
        {
            State = RoundState.Open;
            return OnUpdatedAsync();
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
