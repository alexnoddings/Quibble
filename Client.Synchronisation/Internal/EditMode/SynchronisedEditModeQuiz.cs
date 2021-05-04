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
    internal sealed class SynchronisedEditModeQuiz : SynchronisedEntity, ISynchronisedEditModeQuiz, IDisposable
    {
        private readonly List<IDisposable> _eventHandlers = new();
        private HubConnection HubConnection { get; }

        public Guid Id { get; }
        public Guid OwnerId { get; }
        public string Title { get; private set; }
        public QuizState State { get; private set; }
        public DateTime CreatedAt { get; }
        public DateTime? OpenedAt { get; private set; }

        internal List<SynchronisedEditModeRound> SyncedRounds { get; } = new();
        public IEnumerable<ISynchronisedEditModeRound> Rounds => SyncedRounds.AsEnumerable();

        public SynchronisedEditModeQuiz(HubConnection hubConnection, IQuiz quiz)
        {
            HubConnection = hubConnection;
            Id = quiz.Id;
            OwnerId = quiz.OwnerId;
            Title = quiz.Title;
            State = quiz.State;
            CreatedAt = quiz.CreatedAt;
            OpenedAt = quiz.OpenedAt;

            _eventHandlers.Add(hubConnection.On<string>(nameof(IQuibbleHubClient.OnQuizTitleUpdatedAsync), HandleTitleUpdatedAsync));
            _eventHandlers.Add(hubConnection.On(nameof(IQuibbleHubClient.OnQuizOpenedAsync), HandleOpenedAsync));
            _eventHandlers.Add(hubConnection.On(nameof(IQuibbleHubClient.OnQuizDeletedAsync), HandleDeletedAsync));

            _eventHandlers.Add(hubConnection.On<RoundDto>(nameof(IQuibbleHubClient.OnRoundAddedAsync), HandleRoundAddedAsync));
            _eventHandlers.Add(hubConnection.On<Guid>(nameof(IQuibbleHubClient.OnRoundDeletedAsync), HandleRoundDeletedAsync));
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

        private Task HandleOpenedAsync()
        {
            State = QuizState.Open;
            OpenedAt = DateTime.UtcNow;
            return OnUpdatedAsync();
        }

        private Task HandleDeletedAsync()
        {
            // ToDo: implement this
            return OnUpdatedAsync();
        }

        private Task HandleRoundAddedAsync(RoundDto round)
        {
            var synchronisedRound = new SynchronisedEditModeRound(HubConnection, round);
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

        public void Dispose()
        {
            while (_eventHandlers.Count > 0)
            {
                var handler = _eventHandlers[0];
                handler.Dispose();
                _eventHandlers.Remove(handler);
            }

            while (SyncedRounds.Count > 0)
            {
                var round = SyncedRounds[0];
                round.Dispose();
                SyncedRounds.Remove(round);
            }
        }

        public override int GetStateStamp()
        {
            var hashCode = new HashCode();
            hashCode.Add(Title);
            hashCode.Add(State);
            hashCode.Add(CreatedAt);
            hashCode.Add(OpenedAt);
            foreach (var round in SyncedRounds)
                hashCode.Add(round.GetStateStamp());
            return hashCode.ToHashCode();
        }
    }
}
