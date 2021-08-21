using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities.HostMode
{
    internal sealed class SignalrSyncedHostModeParticipant : SignalrSyncedEntity, ISyncedHostModeParticipant
    {
        public Guid QuizId { get; }
        public string UserName { get; }

        internal SignalrSyncedHostModeQuiz SyncedQuiz { get; }
        public ISyncedHostModeQuiz Quiz => SyncedQuiz;

        internal List<SignalrSyncedHostModeSubmittedAnswer> SyncedAnswers = new();
        public IReadOnlyList<ISyncedHostModeSubmittedAnswer> Answers => SyncedAnswers.AsReadOnly();

        public SignalrSyncedHostModeParticipant(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, ParticipantDto participant, SignalrSyncedHostModeQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = participant.Id;
            QuizId = participant.QuizId;
            UserName = participant.UserName;

            SyncedQuiz = quiz;
        }

        public override int GetStateStamp() =>
            GenerateStateStamp(SyncedAnswers);
    }
}
