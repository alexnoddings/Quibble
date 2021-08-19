using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.Internal.HostMode
{
    internal sealed class SynchronisedHostModeSubmittedAnswer : SignalrSynchronisedEntity, ISynchronisedHostModeSubmittedAnswer
    {
        public Guid QuestionId { get; }
        public Guid ParticipantId { get; }
        public string Text { get; private set; }
        public decimal AssignedPoints { get; private set; }

        internal SynchronisedHostModeQuestion SyncedQuestion { get; }
        public ISynchronisedHostModeQuestion Question => SyncedQuestion;

        internal SynchronisedHostModeParticipant SyncedSubmitter { get; }
        public ISynchronisedHostModeParticipant Submitter => SyncedSubmitter;

        public SynchronisedHostModeSubmittedAnswer(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, ISubmittedAnswer submittedAnswer, SynchronisedHostModeQuestion question, SynchronisedHostModeParticipant participant)
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
