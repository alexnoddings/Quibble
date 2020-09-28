using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;

namespace Quibble.UI.Core.Entities
{
    public sealed class SyncedRound : IRound, IDisposable
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Title { get; set; }

        private readonly List<SyncedQuestion> _questions = new();
        public IReadOnlyList<SyncedQuestion> Questions => _questions;

        private readonly AsyncEvent _updated = new();
        public event Func<Task> Updated
        {
            add => _updated.Add(value);
            remove => _updated.Remove(value);
        }

        private readonly SyncServices _services;

        internal SyncedRound(IRound dtoRound, IEnumerable<SyncedQuestion> questions, SyncServices services)
        {
            _services = services;

            Id = dtoRound.Id;
            QuizId = dtoRound.QuizId;
            Title = dtoRound.Title;
            _questions.AddRange(questions);

            _services.RoundEvents.TitleUpdated += OnRoundTitleUpdatedAsync;
            _services.QuestionEvents.QuestionAdded += OnQuestionAddedAsync;
            _services.QuestionEvents.QuestionDeleted += OnQuestionDeletedAsync;
        }

        internal SyncedRound(IRound dtoRound, SyncServices services)
            : this(dtoRound, Enumerable.Empty<SyncedQuestion>(), services)
        {
        }

        public Task SaveTitleAsync() => _services.RoundService.UpdateTitleAsync(Id, Title);

        public Task AddQuestionAsync() => _services.QuestionService.CreateAsync(Id);

        public Task DeleteAsync() => _services.RoundService.DeleteAsync(Id);

        private Task OnRoundTitleUpdatedAsync(Guid roundId, string newTitle)
        {
            if (roundId != Id) return Task.CompletedTask;

            Title = newTitle;

            return _updated.InvokeAsync();
        }

        private Task OnQuestionAddedAsync(IQuestion question)
        {
            if (question.RoundId != Id) return Task.CompletedTask;

            var syncedQuestion = new SyncedQuestion(question, _services);
            _questions.Add(syncedQuestion);
            return _updated.InvokeAsync();
        }

        private Task OnQuestionDeletedAsync(Guid questionId)
        {
            SyncedQuestion? question = _questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null)
                return Task.CompletedTask;

            _questions.Remove(question);
            return _updated.InvokeAsync();
        }

        public void Dispose()
        {
            _services.RoundEvents.TitleUpdated -= OnRoundTitleUpdatedAsync;
            _services.QuestionEvents.QuestionAdded -= OnQuestionAddedAsync;
            _services.QuestionEvents.QuestionDeleted -= OnQuestionDeletedAsync;

            foreach (var question in _questions)
                question.Dispose();
        }
    }
}
