using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Models;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Internal.HostMode
{
    internal sealed class SynchronisedHostModeQuiz : SignalrSynchronisedEntity, ISynchronisedHostModeQuiz
    {
        public Guid OwnerId { get; }
        public string Title { get; }
        public QuizState State { get; }
        public DateTime CreatedAt { get; }
        public DateTime? OpenedAt { get; }

        internal List<SynchronisedHostModeRound> SyncedRounds { get; } = new();
        public IReadOnlyList<ISynchronisedHostModeRound> Rounds => SyncedRounds.AsReadOnly();

        internal List<SynchronisedHostModeParticipant> SyncedParticipants { get; } = new();
        public IReadOnlyList<ISynchronisedHostModeParticipant> Participants => SyncedParticipants.AsReadOnly();

        public SynchronisedHostModeQuiz(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IQuiz quiz)
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
            var synchronisedParticipant = new SynchronisedHostModeParticipant(Logger, HubConnection, participant, this);
            SyncedParticipants.Add(synchronisedParticipant);

            foreach (var submittedAnswer in submittedAnswers)
            {
                var synchronisedQuestion = SyncedRounds.SelectMany(r => r.SyncedQuestions).First(q => q.Id == submittedAnswer.QuestionId);
                var synchronisedAnswer = new SynchronisedHostModeSubmittedAnswer(Logger, HubConnection, submittedAnswer, synchronisedQuestion, synchronisedParticipant);
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
