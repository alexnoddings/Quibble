using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.TakeMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities.TakeMode
{
    internal sealed class SignalrSyncedTakeModeRound : SignalrSyncedEntity, ISyncedTakeModeRound
    {
        public Guid QuizId { get; }
        public string Title { get; }
        public RoundState State { get; }
        public int Order { get; }

        public event Func<ISyncedTakeModeQuestion, Task>? QuestionAdded;

        internal SignalrSyncedTakeModeQuiz SyncedQuiz { get; }
        public ISyncedTakeModeQuiz Quiz => SyncedQuiz;

        internal List<SignalrSyncedTakeModeQuestion> SyncedQuestions { get; } = new();
        public IReadOnlyList<ISyncedTakeModeQuestion> Questions => SyncedQuestions.AsReadOnly();

        public SignalrSyncedTakeModeRound(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IRound round, SignalrSyncedTakeModeQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = round.Id;
            QuizId = round.QuizId;
            Title = round.Title;
            State = round.State;
            Order = round.Order;

            SyncedQuiz = quiz;

            AddEventHandler<QuestionDto, SubmittedAnswerDto>(c => c.OnQuestionRevealedAsync, HandleQuestionRevealedAsync);
        }

        private async Task HandleQuestionRevealedAsync(QuestionDto question, SubmittedAnswerDto submittedAnswer)
        {
            if (question.RoundId != Id)
                return;

            var synchronisedQuestion = new SignalrSyncedTakeModeQuestion(Logger, HubConnection, question, this);
            var synchronisedAnswer = new SignalrSyncedTakeModeSubmittedAnswer(Logger, HubConnection, submittedAnswer, synchronisedQuestion);
            synchronisedQuestion.SyncedSubmittedAnswer = synchronisedAnswer;

            SyncedQuestions.Add(synchronisedQuestion);
            
            if (QuestionAdded is not null)
                await QuestionAdded.Invoke(synchronisedQuestion);

            await OnUpdatedAsync();
        }

        public override int GetStateStamp() =>
            GenerateStateStamp(State, SyncedQuestions);

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
