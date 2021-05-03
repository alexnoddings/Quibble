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
    internal sealed class SynchronisedEditModeRound : SynchronisedEntity, ISynchronisedEditModeRound, IDisposable
    {
        private readonly List<IDisposable> _eventHandlers = new();
        private HubConnection HubConnection { get; }

        public Guid Id { get; }
        public Guid QuizId { get; }
        public string Title { get; private set; }
        public RoundState State { get; private set; }

        internal List<SynchronisedEditModeQuestion> SyncedQuestions { get; } = new();
        public IEnumerable<ISynchronisedEditModeQuestion> Questions => SyncedQuestions.AsEnumerable();

        public SynchronisedEditModeRound(HubConnection hubConnection, IRound round)
        {
            HubConnection = hubConnection;
            Id = round.Id;
            QuizId = round.QuizId;
            Title = round.Title;
            State = round.State;

            _eventHandlers.Add(hubConnection.On<Guid, string>(nameof(IQuibbleHubClient.OnRoundTitleUpdatedAsync), HandleTitleUpdatedAsync));
            _eventHandlers.Add(hubConnection.On<Guid>(nameof(IQuibbleHubClient.OnRoundOpenedAsync), HandleOpenedAsync));

            _eventHandlers.Add(hubConnection.On<QuestionDto>(nameof(IQuibbleHubClient.OnQuestionAddedAsync), HandleQuestionAddedAsync));
            _eventHandlers.Add(hubConnection.On<Guid>(nameof(IQuibbleHubClient.OnQuestionDeletedAsync), HandleQuestionDeletedAsync));
        }

        public async Task UpdateTitleAsync(string newTitle)
        {
            await HubConnection.InvokeAsync<HubResponse>(Endpoints.UpdateRoundTitle, Id, newTitle);
            Title = newTitle;
        }

        public Task OpenAsync() =>
            HubConnection.InvokeAsync(Endpoints.OpenRound, Id);

        public Task DeleteAsync() =>
            HubConnection.InvokeAsync(Endpoints.DeleteRound, Id);

        public Task AddQuestionAsync() =>
            HubConnection.InvokeAsync(Endpoints.CreateQuestion, Id, string.Empty, string.Empty, 1);

        private Task HandleTitleUpdatedAsync(Guid roundId, string newTitle)
        {
            if (roundId != Id)
                return Task.CompletedTask;

            Title = newTitle;
            return OnUpdatedAsync();
        }

        private Task HandleOpenedAsync(Guid roundId)
        {
            if (roundId != Id)
                return Task.CompletedTask;

            State = RoundState.Open;
            return OnUpdatedAsync();
        }

        private Task HandleQuestionAddedAsync(QuestionDto question)
        {
            if (question.RoundId != Id)
                return Task.CompletedTask;

            var synchronisedQuestion = new SynchronisedEditModeQuestion(HubConnection, question);
            SyncedQuestions.Add(synchronisedQuestion);
            return OnUpdatedAsync();
        }

        private Task HandleQuestionDeletedAsync(Guid questionId)
        {
            SynchronisedEditModeQuestion? question = SyncedQuestions.Find(q => q.Id == questionId);
            if (question is null)
                return Task.CompletedTask;

            SyncedQuestions.Remove(question);
            question.Dispose();
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

            while (SyncedQuestions.Count > 0)
            {
                var question = SyncedQuestions[0];
                question.Dispose();
                SyncedQuestions.Remove(question);
            }
        }
    }
}
