using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.EditMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities.EditMode
{
    internal sealed class SignalrSyncedEditModeQuiz : SignalrSyncedEntity, ISyncedEditModeQuiz
    {
        public event Func<Task>? OnInvalidated;

        public Guid OwnerId { get; }
        public string Title { get; private set; }
        public QuizState State { get; private set; }
        public DateTime CreatedAt { get; }
        public DateTime? OpenedAt { get; private set; }

        internal List<SignalrSyncedEditModeRound> SyncedRounds { get; } = new();
        public IReadOnlyList<ISyncedEditModeRound> Rounds => SyncedRounds.AsReadOnly();

        public SignalrSyncedEditModeQuiz(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = quiz.Id;
            OwnerId = quiz.OwnerId;
            Title = quiz.Title;
            State = quiz.State;
            CreatedAt = quiz.CreatedAt;
            OpenedAt = quiz.OpenedAt;

            AddEventHandler<string>(c => c.OnQuizTitleUpdatedAsync, HandleTitleUpdatedAsync);
            AddEventHandler(c => c.OnQuizOpenedAsync, HandleOpenedAsync);
            AddEventHandler(c => c.OnQuizDeletedAsync, HandleDeletedAsync);

            AddEventHandler<RoundDto>(c => c.OnRoundAddedAsync, HandleRoundAddedAsync);
            AddEventHandler<Guid>(c => c.OnRoundDeletedAsync, HandleRoundDeletedAsync);
        }

        public async Task UpdateTitleAsync(string newTitle)
        {
            var response = await HubConnection.InvokeAsync<HubResponse>(Endpoints.UpdateQuizTitle, newTitle);
            if (response.WasSuccessful)
                Title = newTitle;
        }

        public Task OpenAsync() =>
            HubConnection.InvokeAsync(Endpoints.OpenQuiz);

        public Task DeleteAsync() =>
            HubConnection.InvokeAsync(Endpoints.DeleteQuiz);

        public Task AddRoundAsync() =>
            HubConnection.InvokeAsync(Endpoints.CreateRound, $"Round #{SyncedRounds.Count + 1}");

        private Task HandleTitleUpdatedAsync(string newTitle)
        {
            Title = newTitle;
            return OnUpdatedAsync();
        }

        private async Task HandleOpenedAsync()
        {
            State = QuizState.Open;
            OpenedAt = DateTime.UtcNow;

            // EditMode is no longer valid if the quiz has been opened
            if (OnInvalidated is not null)
                await OnInvalidated.Invoke();
            await OnUpdatedAsync();
        }

        private Task HandleDeletedAsync() =>
            OnInvalidated?.Invoke() ?? Task.CompletedTask;

        private Task HandleRoundAddedAsync(RoundDto round)
        {
            var synchronisedRound = new SignalrSyncedEditModeRound(Logger, HubConnection, round, this);
            SyncedRounds.Add(synchronisedRound);
            return OnUpdatedAsync();
        }

        private Task HandleRoundDeletedAsync(Guid roundId)
        {
            SignalrSyncedEditModeRound? round = SyncedRounds.Find(r => r.Id == roundId);
            if (round is null)
                return Task.CompletedTask;

            SyncedRounds.Remove(round);
            round.Dispose();
            return OnUpdatedAsync();
        }

        public override int GetStateStamp() =>
            GenerateStateStamp(Title, State, OpenedAt, SyncedRounds);

        public async ValueTask DisposeAsync()
        {
            if (IsDisposed) return;

            while (SyncedRounds.Count > 0)
            {
                var round = SyncedRounds[0];
                round.Dispose();
                SyncedRounds.RemoveAt(0);
            }

            // base.Dispose will cause HubConnection to throw a disposed exception
            var hubConnection = HubConnection;
            base.Dispose(true);
            await hubConnection.DisposeAsync();
        }
    }
}
