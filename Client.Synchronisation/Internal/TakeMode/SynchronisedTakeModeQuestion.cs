using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Internal.TakeMode
{
    internal sealed class SynchronisedTakeModeQuestion : SynchronisedEntity, ISynchronisedTakeModeQuestion
    {
        public override Guid Id { get; }
        public Guid RoundId { get; }
        public string Text { get; }
        public string Answer { get; private set; }
        public decimal Points { get; }
        public QuestionState State { get; private set; }
        public int Order { get; }

        internal SynchronisedTakeModeRound SyncedRound { get; }
        public ISynchronisedTakeModeRound Round => SyncedRound;

        internal SynchronisedTakeModeSubmittedAnswer? SyncedSubmittedAnswer { get; set; }
        public ISynchronisedTakeModeSubmittedAnswer? SubmittedAnswer => SyncedSubmittedAnswer;

        public SynchronisedTakeModeQuestion(ILogger<SynchronisedEntity> logger, HubConnection hubConnection, IQuestion question, SynchronisedTakeModeRound round)
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

            AddFilteredEventHandler<string>(c => c.OnQuestionAnswerUpdatedAsync, HandleAnswerUpdatedAsync);
            AddFilteredEventHandler<QuestionState>(c => c.OnQuestionStateUpdatedAsync, HandleStateUpdatedAsync);
        }

        private Task HandleAnswerUpdatedAsync(string newAnswer)
        {
            Answer = newAnswer;
            return OnUpdatedAsync();
        }

        private Task HandleStateUpdatedAsync(QuestionState newState)
        {
            State = newState;
            return OnUpdatedAsync();
        }

        public override int GetStateStamp()
        {
            var hashCode = new HashCode();
            hashCode.Add(Answer);
            hashCode.Add(State);
            hashCode.Add(SyncedSubmittedAnswer?.GetStateStamp() ?? 0);
            return hashCode.ToHashCode();
        }
    }
}
