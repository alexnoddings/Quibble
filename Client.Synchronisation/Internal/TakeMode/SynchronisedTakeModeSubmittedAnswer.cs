using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.Internal.TakeMode
{
    internal sealed class SynchronisedTakeModeSubmittedAnswer : SynchronisedEntity, ISynchronisedTakeModeSubmittedAnswer
    {
        public override Guid Id { get; }
        public Guid QuestionId { get; }
        public Guid ParticipantId { get; }
        public string Text { get; set; }
        public decimal AssignedPoints { get; set; }

        internal SynchronisedTakeModeQuestion SyncedQuestion { get; }
        public ISynchronisedTakeModeQuestion Question => SyncedQuestion;

        public SynchronisedTakeModeSubmittedAnswer(ILogger<SynchronisedEntity> logger, HubConnection hubConnection, ISubmittedAnswer submittedAnswer, SynchronisedTakeModeQuestion question)
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
            await HubConnection.InvokeAsync(Endpoints.PreviewUpdateSubmittedAnswerText, Id, previewText);
            Text = previewText;
        }

        public async Task UpdateTextAsync(string newText)
        {
            await HubConnection.InvokeAsync(Endpoints.UpdateSubmittedAnswerText, Id, newText);
            Text = newText;
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

        public override int GetStateStamp()
        {
            var hashCode = new HashCode();
            hashCode.Add(Text);
            hashCode.Add(AssignedPoints);
            return hashCode.ToHashCode();
        }
    }
}
