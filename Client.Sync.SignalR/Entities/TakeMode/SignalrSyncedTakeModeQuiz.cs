using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities.TakeMode
{
    internal sealed class SignalrSyncedTakeModeQuiz : SignalrSyncedEntity, ISyncedTakeModeQuiz
    {
        public Guid OwnerId { get; }
        public string Title { get; }
        public QuizState State { get; }
        public DateTime CreatedAt { get; }
        public DateTime? OpenedAt { get; }

        public event Func<ISyncedTakeModeRound, Task>? RoundAdded;

        public List<SignalrSyncedTakeModeRound> SyncedRounds { get; } = new();
        public IReadOnlyList<ISyncedTakeModeRound> Rounds => SyncedRounds.AsReadOnly();

        public List<SignalrSyncedTakeModeParticipant> SyncedParticipants { get; } = new();
        public IReadOnlyList<ISyncedTakeModeParticipant> Participants => SyncedParticipants.AsReadOnly();

        public SignalrSyncedTakeModeQuiz(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IQuiz quiz)
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
            var synchronisedRound = new SignalrSyncedTakeModeRound(Logger, HubConnection, round, this);
            SyncedRounds.Add(synchronisedRound);
            if (RoundAdded is not null)
                await RoundAdded.Invoke(synchronisedRound);
            await OnUpdatedAsync();
        }

        private Task HandleParticipantJoinedAsync(ParticipantDto participant, List<SubmittedAnswerDto> _)
        {
            var synchronisedParticipant = new SignalrSyncedTakeModeParticipant(Logger, HubConnection, participant, this);
            SyncedParticipants.Add(synchronisedParticipant);
            return OnUpdatedAsync();
        }

        public override int GetStateStamp() =>
            GenerateStateStamp(SyncedRounds, SyncedParticipants);

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
