using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;

namespace Quibble.UI.Core.Entities
{
    public class SyncedRound : IRound, IDisposable
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Title { get; set; }
        public RoundState State { get; private set; }

        private readonly List<SyncedQuestion> _questions = new();
        public IReadOnlyList<SyncedQuestion> Questions => _questions;

        private readonly AsyncEvent _updated = new();
        public event Func<Task> Updated
        {
            add => _updated.Add(value);
            remove => _updated.Remove(value);
        }

        private readonly SyncServices _services;
        private bool _isDisposed;

        internal SyncedRound(IRound dtoRound, IEnumerable<SyncedQuestion> questions, SyncServices services)
        {
            _services = services;

            Id = dtoRound.Id;
            QuizId = dtoRound.QuizId;
            Title = dtoRound.Title;
            State = dtoRound.State;

            _questions.AddRange(questions);
            foreach (var question in _questions)
                question.Updated += OnQuestionUpdatedInternalAsync;

            _services.RoundEvents.TitleUpdated += OnRoundTitleUpdatedAsync;
            _services.RoundEvents.StateUpdated += OnStateUpdatedAsync;
            _services.QuestionEvents.QuestionAdded += OnQuestionAddedAsync;
            _services.QuestionEvents.QuestionDeleted += OnQuestionDeletedAsync;
        }

        internal SyncedRound(IRound dtoRound, SyncServices services)
            : this(dtoRound, Enumerable.Empty<SyncedQuestion>(), services)
        {
        }

        public Task SaveTitleAsync() => _services.RoundService.UpdateTitleAsync(Id, Title);

        public Task UpdateStateAsync(RoundState newState) => _services.RoundService.UpdateStateAsync(Id, newState);

        public Task AddQuestionAsync() => _services.QuestionService.CreateAsync(Id);

        public Task DeleteAsync() => _services.RoundService.DeleteAsync(Id);

        private Task OnRoundTitleUpdatedAsync(Guid roundId, string newTitle)
        {
            if (roundId != Id) return Task.CompletedTask;

            Title = newTitle;

            return _updated.InvokeAsync();
        }

        private Task OnStateUpdatedAsync(Guid roundId, RoundState newState)
        {
            if (roundId != Id) return Task.CompletedTask;

            State = newState;

            return _updated.InvokeAsync();
        }

        private Task OnQuestionAddedAsync(IQuestion question)
        {
            if (question.RoundId != Id) return Task.CompletedTask;

            var syncedQuestion = new SyncedQuestion(question, _services);
            syncedQuestion.Updated += OnQuestionUpdatedInternalAsync;
            _questions.Add(syncedQuestion);
            return _updated.InvokeAsync();
        }

        private Task OnQuestionDeletedAsync(Guid questionId)
        {
            SyncedQuestion? question = _questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null)
                return Task.CompletedTask;

            question.Updated -= OnQuestionUpdatedInternalAsync;
            _questions.Remove(question);
            return _updated.InvokeAsync();
        }

        private Task OnQuestionUpdatedInternalAsync() => _updated.InvokeAsync();

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _services.RoundEvents.TitleUpdated -= OnRoundTitleUpdatedAsync;
                    _services.RoundEvents.StateUpdated -= OnStateUpdatedAsync;
                    _services.QuestionEvents.QuestionAdded -= OnQuestionAddedAsync;
                    _services.QuestionEvents.QuestionDeleted -= OnQuestionDeletedAsync;

                    foreach (var question in _questions)
                    {
                        question.Updated -= OnQuestionUpdatedInternalAsync;
                        question.Dispose();
                    }
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
