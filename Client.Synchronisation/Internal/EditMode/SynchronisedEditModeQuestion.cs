using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Sync.Entities.EditMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.Internal.EditMode
{
    internal sealed class SynchronisedEditModeQuestion : SynchronisedEntity, ISynchronisedEditModeQuestion, IDisposable
    {
        public Guid Id { get; }
        public Guid RoundId { get; }
        public string Text { get; private set; }
        public string Answer { get; private set; }
        public decimal Points { get; private set; }
        public QuestionState State { get; }

        internal SynchronisedEditModeRound SyncedRound { get; }
        public ISynchronisedEditModeRound Round => SyncedRound;

        public SynchronisedEditModeQuestion(HubConnection hubConnection, IQuestion question, SynchronisedEditModeRound round)
            : base(hubConnection)
        {
            Id = question.Id;
            RoundId = question.RoundId;
            Text = question.Text;
            Answer = question.Answer;
            Points = question.Points;
            State = question.State;

            SyncedRound = round;

            AddEventHandler(hubConnection.On<Guid, string>(nameof(IQuibbleHubClient.OnQuestionTextUpdatedAsync), HandleTextUpdatedAsync));
            AddEventHandler(hubConnection.On<Guid, string>(nameof(IQuibbleHubClient.OnQuestionAnswerUpdatedAsync), HandleAnswerUpdatedAsync));
            AddEventHandler(hubConnection.On<Guid, decimal>(nameof(IQuibbleHubClient.OnQuestionPointsUpdatedAsync), HandlePointsUpdatedAsync));
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

        public Task UpdatePointsAsync(decimal newPoints) =>
            HubConnection.InvokeAsync(Endpoints.UpdateQuestionPoints, Id, newPoints);

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

        private Task HandlePointsUpdatedAsync(Guid questionId, decimal newPoints)
        {
            if (questionId != Id)
                return Task.CompletedTask;

            Points = newPoints;
            return OnUpdatedAsync();
        }

        public override int GetStateStamp()
        {
            var hashCode = new HashCode();
            hashCode.Add(Text);
            hashCode.Add(Answer);
            hashCode.Add(Points);
            return hashCode.ToHashCode();
        }
    }
}
