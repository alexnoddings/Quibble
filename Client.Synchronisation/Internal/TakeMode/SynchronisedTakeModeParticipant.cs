using System;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Models;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Internal.TakeMode
{
    internal sealed class SynchronisedTakeModeParticipant : SignalrSynchronisedEntity, ISynchronisedTakeModeParticipant
    {
        public Guid QuizId { get; }
        public string UserName { get; }

        internal SynchronisedTakeModeQuiz SyncedQuiz { get; }
        public ISynchronisedTakeModeQuiz Quiz => SyncedQuiz;

        public SynchronisedTakeModeParticipant(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, ParticipantDto participant, SynchronisedTakeModeQuiz quiz)
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
