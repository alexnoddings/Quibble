using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.SignalR.Entities.HostMode
{
    internal sealed class SignalrSyncedHostModeSubmittedAnswer : SignalrSyncedEntity, ISyncedHostModeSubmittedAnswer
    {
        public Guid QuestionId { get; }
        public Guid ParticipantId { get; }
        public string Text { get; private set; }
        public decimal AssignedPoints { get; private set; }

        internal SignalrSyncedHostModeQuestion SyncedQuestion { get; }
        public ISyncedHostModeQuestion Question => SyncedQuestion;

        internal SignalrSyncedHostModeParticipant SyncedSubmitter { get; }
        public ISyncedHostModeParticipant Submitter => SyncedSubmitter;

        public SignalrSyncedHostModeSubmittedAnswer(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, ISubmittedAnswer submittedAnswer, SignalrSyncedHostModeQuestion question, SignalrSyncedHostModeParticipant participant)
            : base(logger, hubConnection)
        {
            Id = submittedAnswer.Id;
            QuestionId = submittedAnswer.QuestionId;
            ParticipantId = submittedAnswer.ParticipantId;
            Text = submittedAnswer.Text;
            AssignedPoints = submittedAnswer.AssignedPoints;

            SyncedQuestion = question;
            SyncedSubmitter = participant;

            AddFilteredEventHandler<string>(c => c.OnSubmittedAnswerTextUpdatedAsync, HandleTextUpdatedAsync);
            AddFilteredEventHandler<decimal>(c => c.OnSubmittedAnswerAssignedPointsUpdatedAsync, HandleAssignedPointsUpdatedAsync);
        }

        public Task MarkAsync(decimal points)
        {
            if (SyncedQuestion.State < QuestionState.Locked)
                return Task.CompletedTask;

            return HubConnection.InvokeAsync(Endpoints.UpdateSubmittedAnswerAssignedPoints, Id, points);
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
