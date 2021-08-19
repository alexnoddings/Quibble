using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.EditMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.Internal.EditMode
{
    internal sealed class SynchronisedEditModeQuestion : SignalrSynchronisedEntity, ISynchronisedEditModeQuestion
    {
        public Guid RoundId { get; }
        public string Text { get; private set; }
        public string Answer { get; private set; }
        public decimal Points { get; private set; }
        public QuestionState State { get; }
        public int Order { get; } = 0;

        internal SynchronisedEditModeRound SyncedRound { get; }
        public ISynchronisedEditModeRound Round => SyncedRound;

        public SynchronisedEditModeQuestion(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IQuestion question, SynchronisedEditModeRound round)
            : base(logger, hubConnection)
        {
            Id = question.Id;
            RoundId = question.RoundId;
            Text = question.Text;
            Answer = question.Answer;
            Points = question.Points;
            State = question.State;

            SyncedRound = round;

            AddFilteredEventHandler<string>(c => c.OnQuestionTextUpdatedAsync, HandleTextUpdatedAsync);
            AddFilteredEventHandler<string>(c => c.OnQuestionAnswerUpdatedAsync, HandleAnswerUpdatedAsync);
            AddFilteredEventHandler<decimal>(c => c.OnQuestionPointsUpdatedAsync, HandlePointsUpdatedAsync);
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

        public override int GetStateStamp() =>
            GenerateStateStamp(Text, Answer, Points);
    }
}
