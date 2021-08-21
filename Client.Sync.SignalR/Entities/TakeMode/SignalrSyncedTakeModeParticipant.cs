using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities.TakeMode
{
    internal sealed class SignalrSyncedTakeModeParticipant : SignalrSyncedEntity, ISyncedTakeModeParticipant
    {
        public Guid QuizId { get; }
        public string UserName { get; }

        internal SignalrSyncedTakeModeQuiz SyncedQuiz { get; }
        public ISyncedTakeModeQuiz Quiz => SyncedQuiz;

        public SignalrSyncedTakeModeParticipant(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, ParticipantDto participant, SignalrSyncedTakeModeQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = participant.Id;
            QuizId = participant.QuizId;
            UserName = participant.UserName;

            SyncedQuiz = quiz;
        }

        // No fields are changed, and we have no child entities
        public override int GetStateStamp() => 0;
    }
}
