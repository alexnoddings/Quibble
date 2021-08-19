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
    internal sealed class SynchronisedHostModeQuestion : SignalrSynchronisedEntity, ISynchronisedHostModeQuestion
    {
        public Guid RoundId { get; }
        public string Text { get; }
        public string Answer { get; }
        public decimal Points { get; }
        public QuestionState State { get; private set; }
        public int Order { get; }

        internal SynchronisedHostModeRound SyncedRound { get; }
        public ISynchronisedHostModeRound Round => SyncedRound;

        internal List<SynchronisedHostModeSubmittedAnswer> SyncedAnswers { get; } = new();
        public IReadOnlyList<ISynchronisedHostModeSubmittedAnswer> Answers => SyncedAnswers.AsReadOnly();

        public SynchronisedHostModeQuestion(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IQuestion question, SynchronisedHostModeRound round)
            : base(logger, hubConnection)
        {
            Id = question.Id;
            RoundId = question.RoundId;
            Text = question.Text;
            Answer = question.Answer;
            Points = question.Points;
            State = question.State;
            Order = question.Order;

            SyncedRound = round;

            AddFilteredEventHandler<QuestionState>(c => c.OnQuestionStateUpdatedAsync, HandleStateUpdatedAsync);
        }

        public Task OpenAsync()
        {
            if (State != QuestionState.Hidden)
                return Task.CompletedTask;

            return HubConnection.InvokeAsync(Endpoints.UpdateQuestionState, Id, QuestionState.Open);
        }

        public Task LockAsync()
        {
            if (State != QuestionState.Open)
                return Task.CompletedTask;

            return HubConnection.InvokeAsync(Endpoints.UpdateQuestionState, Id, QuestionState.Locked);
        }

        public Task ShowAnswer()
        {
            if (State != QuestionState.Locked)
                return Task.CompletedTask;

            return HubConnection.InvokeAsync(Endpoints.UpdateQuestionState, Id, QuestionState.AnswerRevealed);
        }

        private Task HandleStateUpdatedAsync(QuestionState newState)
        {
            State = newState;
            return OnUpdatedAsync();
        }

        public override int GetStateStamp()
            => GenerateStateStamp(State, SyncedAnswers);

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                while (SyncedAnswers.Count > 0)
                {
                    var answer = SyncedAnswers[0];
                    answer.Dispose();
                    SyncedAnswers.RemoveAt(0);
                }
            }

            base.Dispose(disposing);
        }
    }
}
