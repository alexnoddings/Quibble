using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Models;

namespace Quibble.Client.Sync.Internal.TakeMode
{
    internal sealed class SynchronisedTakeModeQuiz : SynchronisedEntity, ISynchronisedTakeModeQuiz
    {
        public override Guid Id { get; }
        public Guid OwnerId { get; }
        public string Title { get; }
        public QuizState State { get; }
        public DateTime CreatedAt { get; }
        public DateTime? OpenedAt { get; }

        public event Func<ISynchronisedTakeModeRound, Task>? RoundAdded;

        public List<SynchronisedTakeModeRound> SyncedRounds { get; } = new();
        public IReadOnlyList<ISynchronisedTakeModeRound> Rounds => SyncedRounds.AsReadOnly();

        public List<SynchronisedTakeModeParticipant> SyncedParticipants { get; } = new();
        public IReadOnlyList<ISynchronisedTakeModeParticipant> Participants => SyncedParticipants.AsReadOnly();

        public SynchronisedTakeModeQuiz(ILogger<SynchronisedEntity> logger, HubConnection hubConnection, IQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = quiz.Id;
            OwnerId = quiz.OwnerId;
            Title = quiz.Title;
            State = quiz.State;
            CreatedAt = quiz.CreatedAt;
            OpenedAt = quiz.OpenedAt;

            AddEventHandler<RoundDto>(c => c.OnRoundAddedAsync, HandleRoundAddedAsync);
            AddEventHandler<ParticipantDto, List<SubmittedAnswerDto>>(c => c.OnParticipantJoinedAsync, HandleParticipantJoinedAsync);
        }

        private async Task HandleRoundAddedAsync(RoundDto round)
        {
            var synchronisedRound = new SynchronisedTakeModeRound(Logger, HubConnection, round, this);
            SyncedRounds.Add(synchronisedRound);
            if (RoundAdded is not null)
                await RoundAdded.Invoke(synchronisedRound);
            await OnUpdatedAsync();
        }

        private Task HandleParticipantJoinedAsync(ParticipantDto participant, List<SubmittedAnswerDto> _)
        {
            var synchronisedParticipant = new SynchronisedTakeModeParticipant(Logger, HubConnection, participant, this);
            SyncedParticipants.Add(synchronisedParticipant);
            return OnUpdatedAsync();
        }

        public override int GetStateStamp()
        {
            var hashCode = new HashCode();
            foreach (var round in SyncedRounds)
                hashCode.Add(round.GetStateStamp());
            foreach (var participant in SyncedParticipants)
                hashCode.Add(participant.GetStateStamp());
            return hashCode.ToHashCode();
        }

        public async ValueTask DisposeAsync()
        {
            if (IsDisposed) return;

            while (SyncedRounds.Count > 0)
            {
                var round = SyncedRounds[0];
                round.Dispose();
                SyncedRounds.RemoveAt(0);
            }

            while (SyncedParticipants.Count > 0)
            {
                var participant = SyncedParticipants[0];
                participant.Dispose();
                SyncedParticipants.RemoveAt(0);
            }

            // base.Dispose will cause HubConnection to throw a disposed exception
            var hubConnection = HubConnection;
            base.Dispose(true);
            await hubConnection.DisposeAsync();
        }
    }
}
