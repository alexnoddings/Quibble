using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;

namespace Quibble.UI.Core.Entities
{
    public class SyncedQuestion : IQuestion, IDisposable
    {
        private Guid Token { get; } = Guid.NewGuid();

        public Guid Id { get; set; }
        public Guid RoundId { get; set; }
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }

        private readonly List<SyncedAnswer> _answers = new();
        public IReadOnlyList<SyncedAnswer> Answers => _answers;

        public QuestionState State { get; private set; }

        private readonly AsyncEvent _updated = new();
        public event Func<Task> Updated
        {
            add => _updated.Add(value);
            remove => _updated.Remove(value);
        }

        private readonly SyncServices _services;
        private bool _isDisposed;

        internal SyncedQuestion(IQuestion dtoQuestion, IEnumerable<SyncedAnswer> answers, SyncServices services)
        {
            _services = services;

            Id = dtoQuestion.Id;
            RoundId = dtoQuestion.RoundId;
            QuestionText = dtoQuestion.QuestionText;
            CorrectAnswer = dtoQuestion.CorrectAnswer;
            State = dtoQuestion.State;

            _answers.AddRange(answers);
            foreach (var answer in _answers)
                answer.Updated += OnAnswerUpdatedInternalAsync;

            _services.QuestionEvents.CorrectAnswerUpdated += OnQuestionAnswerUpdatedAsync;
            _services.QuestionEvents.TextUpdated += OnQuestionTextUpdatedAsync;
            _services.QuestionEvents.StateUpdated += OnQuestionStateUpdatedAsync;

            _services.AnswerEvents.NewAnswerSubmitted += OnNewAnswerSubmittedAsync;

            _services.ParticipantEvents.ParticipantLeft += OnParticipantLeftAsync;
        }

        internal SyncedQuestion(IQuestion dtoQuestion, SyncServices services)
            : this(dtoQuestion, Enumerable.Empty<SyncedAnswer>(), services)
        {
        }

        public Task SaveTextAsync() => _services.QuestionService.UpdateTextAsync(Id, QuestionText, Token);

        public Task SaveCorrectAnswerAsync() => _services.QuestionService.UpdateCorrectAnswerAsync(Id, CorrectAnswer, Token);

        public Task UpdateStateAsync(QuestionState newState) => _services.QuestionService.UpdateStateAsync(Id, newState);

        public Task DeleteAsync() =>_services.QuestionService.DeleteAsync(Id);

        private Task OnQuestionTextUpdatedAsync(Guid questionId, string newText, Guid initiatorToken)
        {
            if (Token == initiatorToken) return Task.CompletedTask;
            if (questionId != Id) return Task.CompletedTask;

            QuestionText = newText;

            return _updated.InvokeAsync();
        }

        private Task OnQuestionAnswerUpdatedAsync(Guid questionId, string newAnswer, Guid initiatorToken)
        {
            if (Token == initiatorToken) return Task.CompletedTask;
            if (questionId != Id) return Task.CompletedTask;

            CorrectAnswer = newAnswer;

            return _updated.InvokeAsync();
        }

        private Task OnQuestionStateUpdatedAsync(Guid questionId, QuestionState newState)
        {
            if (questionId != Id) return Task.CompletedTask;

            State = newState;

            return _updated.InvokeAsync();
        }

        private Task OnNewAnswerSubmittedAsync(IParticipantAnswer answer)
        {
            if (answer.QuestionId != Id) return Task.CompletedTask;

            var syncedAnswer = new SyncedAnswer(answer, _services);
            syncedAnswer.Updated += OnAnswerUpdatedInternalAsync;
            _answers.Add(syncedAnswer);
            return _updated.InvokeAsync();
        }

        private Task OnParticipantLeftAsync(Guid id)
        {
            var answer = _answers.Find(a => a.ParticipantId == id);
            if (answer == null) 
                return Task.CompletedTask;

            answer.Updated -= OnAnswerUpdatedInternalAsync;
            _answers.Remove(answer);
            return _updated.InvokeAsync();
        }

        private Task OnAnswerUpdatedInternalAsync() => _updated.InvokeAsync();

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _services.QuestionEvents.CorrectAnswerUpdated -= OnQuestionAnswerUpdatedAsync;
                    _services.QuestionEvents.TextUpdated -= OnQuestionTextUpdatedAsync;
                    _services.QuestionEvents.StateUpdated -= OnQuestionStateUpdatedAsync;

                    _services.AnswerEvents.NewAnswerSubmitted -= OnNewAnswerSubmittedAsync;

                    _services.ParticipantEvents.ParticipantLeft -= OnParticipantLeftAsync;

                    foreach (var answer in _answers)
                    {
                        answer.Updated -= OnAnswerUpdatedInternalAsync;
                        answer.Dispose();
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
