using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Sync.Entities.EditMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models;

namespace Quibble.Client.Sync.Internal.EditMode
{
    internal sealed class SynchronisedEditModeQuestion : SynchronisedEntity, ISynchronisedEditModeQuestion, IDisposable
    {
        private readonly List<IDisposable> _eventHandlers = new();
        private HubConnection HubConnection { get; }

        public Guid Id { get; }
        public Guid RoundId { get; }
        public string Text { get; private set; }
        public string Answer { get; private set; }
        public sbyte Points { get; private set; }
        public QuestionState State { get; private set; }

        public SynchronisedEditModeQuestion(HubConnection hubConnection, IQuestion question)
        {
            HubConnection = hubConnection;
            Id = question.Id;
            RoundId = question.RoundId;
            Text = question.Text;
            Answer = question.Answer;
            Points = question.Points;
            State = question.State;

            _eventHandlers.Add(hubConnection.On<Guid, string>(nameof(IQuibbleHubClient.OnQuestionTextUpdatedAsync), HandleTextUpdatedAsync));
            _eventHandlers.Add(hubConnection.On<Guid, string>(nameof(IQuibbleHubClient.OnQuestionAnswerUpdatedAsync), HandleAnswerUpdatedAsync));
            _eventHandlers.Add(hubConnection.On<Guid, sbyte>(nameof(IQuibbleHubClient.OnQuestionPointsUpdatedAsync), HandlePointsUpdatedAsync));
            _eventHandlers.Add(hubConnection.On<Guid, QuestionState>(nameof(IQuibbleHubClient.OnQuestionStateUpdatedAsync), HandleStateUpdatedAsync));
        }

        public async Task UpdateTextAsync(string newText)
        {
            await HubConnection.InvokeAsync(Endpoints.UpdateQuestionText, Id, newText);
            Text = newText;
        }

        public async Task UpdateAnswerAsync(string newAnswer)
        {
            await HubConnection.InvokeAsync(Endpoints.UpdateQuestionAnswer, Id, newAnswer);
            Answer = newAnswer;
        }

        public Task UpdatePointsAsync(sbyte newPoints) =>
            HubConnection.InvokeAsync(Endpoints.UpdateQuestionPoints, Id, newPoints);

        public Task UpdateStateAsync(QuestionState newState) =>
            HubConnection.InvokeAsync(Endpoints.UpdateQuestionState, Id, newState);

        public Task DeleteAsync() =>
            HubConnection.InvokeAsync(Endpoints.DeleteQuestion, Id);

        private Task HandleTextUpdatedAsync(Guid questionId, string newText)
        {
            if (questionId != Id)
                return Task.CompletedTask;

            Text = newText;
            return OnUpdatedAsync();
        }

        private Task HandleAnswerUpdatedAsync(Guid questionId, string newAnswer)
        {
            if (questionId != Id)
                return Task.CompletedTask;

            Answer = newAnswer;
            return OnUpdatedAsync();
        }

        private Task HandlePointsUpdatedAsync(Guid questionId, sbyte newPoints)
        {
            if (questionId != Id)
                return Task.CompletedTask;

            Points = newPoints;
            return OnUpdatedAsync();
        }

        private Task HandleStateUpdatedAsync(Guid questionId, QuestionState newState)
        {
            if (questionId != Id)
                return Task.CompletedTask;

            State = newState;
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
        }
    }
}
