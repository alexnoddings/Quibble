using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Sync.Entities.EditMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models;

namespace Quibble.Client.Sync.Internal.EditMode
{
    internal sealed class SynchronisedEditModeQuiz : SynchronisedEntity, ISynchronisedEditModeQuiz
    {
        public event Func<Task>? OnInvalidated;

        public Guid Id { get; }
        public override Guid Id { get; }
        public Guid OwnerId { get; }
        public string Title { get; private set; }
        public QuizState State { get; private set; }
        public DateTime CreatedAt { get; }
        public DateTime? OpenedAt { get; private set; }

        internal List<SynchronisedEditModeRound> SyncedRounds { get; } = new();
        public IEnumerable<ISynchronisedEditModeRound> Rounds => SyncedRounds.AsEnumerable();

        public SynchronisedEditModeQuiz(HubConnection hubConnection, IQuiz quiz) 
	        : base(hubConnection)
        {
            Id = quiz.Id;
            OwnerId = quiz.OwnerId;
            Title = quiz.Title;
            State = quiz.State;
            CreatedAt = quiz.CreatedAt;
            OpenedAt = quiz.OpenedAt;

            AddEventHandler(hubConnection.On<string>(nameof(IQuibbleHubClient.OnQuizTitleUpdatedAsync), HandleTitleUpdatedAsync));
            AddEventHandler(hubConnection.On(nameof(IQuibbleHubClient.OnQuizOpenedAsync), HandleOpenedAsync));
            AddEventHandler(hubConnection.On(nameof(IQuibbleHubClient.OnQuizDeletedAsync), HandleDeletedAsync));

            AddEventHandler(hubConnection.On<RoundDto>(nameof(IQuibbleHubClient.OnRoundAddedAsync), HandleRoundAddedAsync));
            AddEventHandler(hubConnection.On<Guid>(nameof(IQuibbleHubClient.OnRoundDeletedAsync), HandleRoundDeletedAsync));
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
            var synchronisedRound = new SynchronisedEditModeRound(HubConnection, round, this);
            SyncedRounds.Add(synchronisedRound);
            return OnUpdatedAsync();
        }

        private Task HandleRoundDeletedAsync(Guid roundId)
        {
            SynchronisedEditModeRound? round = SyncedRounds.Find(r => r.Id == roundId);
            if (round is null)
                return Task.CompletedTask;

            SyncedRounds.Remove(round);
            round.Dispose();
            return OnUpdatedAsync();
        }

        public override int GetStateStamp()
        {
            var hashCode = new HashCode();
            hashCode.Add(Title);
            hashCode.Add(State);
            hashCode.Add(OpenedAt);
            foreach (var round in SyncedRounds)
                hashCode.Add(round.GetStateStamp());
            return hashCode.ToHashCode();
        }

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
