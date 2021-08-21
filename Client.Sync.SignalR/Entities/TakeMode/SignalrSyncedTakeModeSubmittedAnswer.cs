using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.SignalR.Entities.TakeMode
{
    internal sealed class SignalrSyncedTakeModeSubmittedAnswer : SignalrSyncedEntity, ISyncedTakeModeSubmittedAnswer
    {
        public Guid QuestionId { get; }
        public Guid ParticipantId { get; }
        public string Text { get; set; }
        public decimal AssignedPoints { get; set; }

        internal SignalrSyncedTakeModeQuestion SyncedQuestion { get; }
        public ISyncedTakeModeQuestion Question => SyncedQuestion;

        public SignalrSyncedTakeModeSubmittedAnswer(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, ISubmittedAnswer submittedAnswer, SignalrSyncedTakeModeQuestion question)
            : base(logger, hubConnection)
        {
            Id = submittedAnswer.Id;
            QuestionId = submittedAnswer.QuestionId;
            ParticipantId = submittedAnswer.ParticipantId;
            Text = submittedAnswer.Text;
            AssignedPoints = submittedAnswer.AssignedPoints;

            SyncedQuestion = question;

            AddFilteredEventHandler<string>(c => c.OnSubmittedAnswerTextUpdatedAsync, HandleTextUpdatedAsync);
            AddFilteredEventHandler<decimal>(c => c.OnSubmittedAnswerAssignedPointsUpdatedAsync, HandleAssignedPointsUpdatedAsync);
        }

        public async Task PreviewUpdateTextAsync(string previewText)
        {
            Text = previewText;
            await HubConnection.InvokeAsync(Endpoints.PreviewUpdateSubmittedAnswerText, Id, previewText);
        }

        public async Task UpdateTextAsync(string newText)
        {
            Text = newText;
            await HubConnection.InvokeAsync(Endpoints.UpdateSubmittedAnswerText, Id, newText);
        }

        private Task HandleTextUpdatedAsync(string newText)
        {
            Text = newText;
            return OnUpdatedAsync();
        }

        private Task HandleAssignedPointsUpdatedAsync(decimal newPoints)
        {
            AssignedPoints = newPoints;
            return OnUpdatedAsync();
        }

        public override int GetStateStamp() =>
            GenerateStateStamp(Text, AssignedPoints);
    }
}
