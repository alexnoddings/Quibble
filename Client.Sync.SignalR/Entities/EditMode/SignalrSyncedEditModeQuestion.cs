using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.EditMode;
using Quibble.Shared.Api;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.SignalR.Entities.EditMode
{
    internal sealed class SignalrSyncedEditModeQuestion : SignalrSyncedEntity, ISyncedEditModeQuestion
    {
        public Guid RoundId { get; }
        public string Text { get; private set; }
        public string Answer { get; private set; }
        public decimal Points { get; private set; }
        public QuestionState State { get; }
        public int Order { get; private set; }

        internal SignalrSyncedEditModeRound SyncedRound { get; }
        public ISyncedEditModeRound Round => SyncedRound;

        public SignalrSyncedEditModeQuestion(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IQuestion question, SignalrSyncedEditModeRound round)
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

            AddFilteredEventHandler<string>(c => c.OnQuestionTextUpdatedAsync, HandleTextUpdatedAsync);
            AddFilteredEventHandler<string>(c => c.OnQuestionAnswerUpdatedAsync, HandleAnswerUpdatedAsync);
            AddFilteredEventHandler<decimal>(c => c.OnQuestionPointsUpdatedAsync, HandlePointsUpdatedAsync);
            AddFilteredEventHandler<int>(c => c.OnQuestionOrderUpdatedAsync, HandleOrderUpdatedAsync);
        }

        public async Task UpdateTextAsync(string newText)
        {
            var response = await HubConnection.InvokeAsync<ApiResponse>(Endpoints.UpdateQuestionText, Id, newText);
            if (response.WasSuccessful)
                Text = newText;
        }

        public async Task UpdateAnswerAsync(string newAnswer)
        {
            var response = await HubConnection.InvokeAsync<ApiResponse>(Endpoints.UpdateQuestionAnswer, Id, newAnswer);
            if (response.WasSuccessful)
                Answer = newAnswer;
        }

        public Task UpdatePointsAsync(decimal newPoints) =>
            HubConnection.InvokeAsync(Endpoints.UpdateQuestionPoints, Id, newPoints);

        public Task DeleteAsync() =>
            HubConnection.InvokeAsync(Endpoints.DeleteQuestion, Id);

        private Task HandleTextUpdatedAsync(string newText)
        {
            Text = newText;
            return OnUpdatedAsync();
        }

        private Task HandleAnswerUpdatedAsync(string newAnswer)
        {
            Answer = newAnswer;
            return OnUpdatedAsync();
        }

        private Task HandlePointsUpdatedAsync(decimal newPoints)
        {
            Points = newPoints;
            return OnUpdatedAsync();
        }

        private Task HandleOrderUpdatedAsync(int newOrder)
        {
            Order = newOrder;
            return OnUpdatedAsync();
        }

        public override int GetStateStamp() =>
            GenerateStateStamp(Text, Answer, Points, Order);
    }
}
