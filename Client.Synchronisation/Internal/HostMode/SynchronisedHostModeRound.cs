using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.HostMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.Internal.HostMode
{
    internal sealed class SynchronisedHostModeRound : SynchronisedEntity, ISynchronisedHostModeRound
    {
        public override Guid Id { get; }
        public Guid QuizId { get; }
        public string Title { get; }
        public RoundState State { get; private set; }

        internal SynchronisedHostModeQuiz SyncedQuiz { get; }
        public ISynchronisedHostModeQuiz Quiz => SyncedQuiz;

        internal List<SynchronisedHostModeQuestion> SyncedQuestions { get; } = new();
        public IReadOnlyList<ISynchronisedHostModeQuestion> Questions => SyncedQuestions.AsReadOnly();

        public SynchronisedHostModeRound(ILogger<SynchronisedEntity> logger, HubConnection hubConnection, IRound round, SynchronisedHostModeQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = round.Id;
            QuizId = round.QuizId;
            Title = round.Title;
            State = round.State;

            SyncedQuiz = quiz;

            AddFilteredEventHandler(c => c.OnRoundOpenedAsync, HandleRoundOpenedAsync);
        }

        public Task OpenAsync()
        {
            if (State != RoundState.Hidden)
                return Task.CompletedTask;

            return HubConnection.InvokeAsync(Endpoints.OpenRound, Id);
        }

        private Task HandleRoundOpenedAsync()
        {
            State = RoundState.Open;
            return OnUpdatedAsync();
        }

        public override int GetStateStamp()
        {
            var hashCode = new HashCode();
            hashCode.Add(State);
            foreach (var question in SyncedQuestions)
                hashCode.Add(question.GetStateStamp());
            return hashCode.ToHashCode();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                while (SyncedQuestions.Count > 0)
                {
                    var question = SyncedQuestions[0];
                    question.Dispose();
                    SyncedQuestions.RemoveAt(0);
                }
            }

            base.Dispose(disposing);
        }
    }
}
