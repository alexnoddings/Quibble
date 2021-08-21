using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Entities.EditMode;
using Quibble.Shared.Entities;
using Quibble.Shared.Hub;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Internal.EditMode
{
    internal sealed class SynchronisedEditModeRound : SignalrSynchronisedEntity, ISynchronisedEditModeRound
    {
        public Guid QuizId { get; }
        public string Title { get; private set; }
        public RoundState State { get; }
        public int Order { get; private set; }

        internal SynchronisedEditModeQuiz SyncedQuiz { get; }
        public ISynchronisedEditModeQuiz Quiz => SyncedQuiz;

        internal List<SynchronisedEditModeQuestion> SyncedQuestions { get; } = new();
        public IReadOnlyList<ISynchronisedEditModeQuestion> Questions => SyncedQuestions.AsReadOnly();

        public SynchronisedEditModeRound(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection, IRound round, SynchronisedEditModeQuiz quiz)
            : base(logger, hubConnection)
        {
            Id = round.Id;
            QuizId = round.QuizId;
            Title = round.Title;
            State = round.State;
            Order = round.Order;

            SyncedQuiz = quiz;

            AddFilteredEventHandler<string>(c => c.OnRoundTitleUpdatedAsync, HandleTitleUpdatedAsync);
            AddFilteredEventHandler<int>(c => c.OnRoundOrderUpdatedAsync, HandleRoundOrderUpdatedAsync);

            AddEventHandler<QuestionDto>(c => c.OnQuestionAddedAsync, HandleQuestionAddedAsync);
            AddEventHandler<Guid>(c => c.OnQuestionDeletedAsync, HandleQuestionDeletedAsync);
        }

        public async Task UpdateTitleAsync(string newTitle)
        {
            await HubConnection.InvokeAsync<HubResponse>(Endpoints.UpdateRoundTitle, Id, newTitle);
            Title = newTitle;
        }

        public Task DeleteAsync() =>
            HubConnection.InvokeAsync(Endpoints.DeleteRound, Id);

        public Task AddQuestionAsync() =>
            HubConnection.InvokeAsync(Endpoints.CreateQuestion, Id, string.Empty, string.Empty, 1);

        private Task HandleTitleUpdatedAsync(string newTitle)
        {
            Title = newTitle;
            return OnUpdatedAsync();
        }

        private Task HandleRoundOrderUpdatedAsync(int newOrder)
        {
            Order = newOrder;
            Console.WriteLine(Title + " now " + newOrder.ToString());
            return OnUpdatedAsync();
        }

        private Task HandleQuestionAddedAsync(QuestionDto question)
        {
            if (question.RoundId != Id)
                return Task.CompletedTask;

            var synchronisedQuestion = new SynchronisedEditModeQuestion(Logger, HubConnection, question, this);
            SyncedQuestions.Add(synchronisedQuestion);
            return OnUpdatedAsync();
        }

        private Task HandleQuestionDeletedAsync(Guid questionId)
        {
            SynchronisedEditModeQuestion? question = SyncedQuestions.Find(q => q.Id == questionId);
            if (question is null)
                return Task.CompletedTask;

            SyncedQuestions.Remove(question);
            question.Dispose();
            return OnUpdatedAsync();
        }

        public override int GetStateStamp() =>
            GenerateStateStamp(Title, Order, SyncedQuestions);

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
