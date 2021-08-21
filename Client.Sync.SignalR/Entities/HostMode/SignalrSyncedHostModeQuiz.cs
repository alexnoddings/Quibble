using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities.HostMode
{
    internal sealed class SignalrSyncedHostModeQuiz : SignalrSyncedEntity, ISyncedHostModeQuiz
    {
        public Guid OwnerId { get; }
        public string Title { get; }
        public QuizState State { get; }
        public DateTime CreatedAt { get; }
        public DateTime? OpenedAt { get; }

        internal List<SignalrSyncedHostModeRound> SyncedRounds { get; } = new();
        public IReadOnlyList<ISyncedHostModeRound> Rounds => SyncedRounds.AsReadOnly();

        internal List<SignalrSyncedHostModeParticipant> SyncedParticipants { get; } = new();
        public IReadOnlyList<ISyncedHostModeParticipant> Participants => SyncedParticipants.AsReadOnly();

        public SignalrSyncedHostModeQuiz(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = quiz.Id;
            OwnerId = quiz.OwnerId;
            Title = quiz.Title;
            State = quiz.State;
            CreatedAt = quiz.CreatedAt;
            OpenedAt = quiz.OpenedAt;

            AddEventHandler<ParticipantDto, List<SubmittedAnswerDto>>(c => c.OnParticipantJoinedAsync, HandleParticipantJoinedAsync);
        }

        private Task HandleParticipantJoinedAsync(ParticipantDto participant, List<SubmittedAnswerDto> submittedAnswers)
        {
            var synchronisedParticipant = new SignalrSyncedHostModeParticipant(Logger, HubConnection, participant, this);
            SyncedParticipants.Add(synchronisedParticipant);

            foreach (var submittedAnswer in submittedAnswers)
            {
                var synchronisedQuestion = SyncedRounds.SelectMany(r => r.SyncedQuestions).First(q => q.Id == submittedAnswer.QuestionId);
                var synchronisedAnswer = new SignalrSyncedHostModeSubmittedAnswer(Logger, HubConnection, submittedAnswer, synchronisedQuestion, synchronisedParticipant);
                synchronisedQuestion.SyncedAnswers.Add(synchronisedAnswer);
                synchronisedParticipant.SyncedAnswers.Add(synchronisedAnswer);
            }

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
