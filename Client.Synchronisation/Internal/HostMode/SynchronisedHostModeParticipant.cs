using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Models;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Internal.HostMode
{
    internal sealed class SynchronisedHostModeParticipant : SynchronisedEntity, ISynchronisedHostModeParticipant
    {
        public override Guid Id { get; }
        public Guid QuizId { get; }
        public string UserName { get; }

        internal SynchronisedHostModeQuiz SyncedQuiz { get; }
        public ISynchronisedHostModeQuiz Quiz => SyncedQuiz;

        internal List<SynchronisedHostModeSubmittedAnswer> SyncedAnswers = new();
        public IReadOnlyList<ISynchronisedHostModeSubmittedAnswer> Answers => SyncedAnswers.AsReadOnly();

        public SynchronisedHostModeParticipant(ILogger<SynchronisedEntity> logger, HubConnection hubConnection, ParticipantDto participant, SynchronisedHostModeQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = participant.Id;
            QuizId = participant.QuizId;
            UserName = participant.UserName;

            SyncedQuiz = quiz;
        }

        public override int GetStateStamp()
        {
            var hashCode = new HashCode();
            foreach (var answer in SyncedAnswers)
                hashCode.Add(answer.GetStateStamp());
            return hashCode.ToHashCode();
        }
    }
}
