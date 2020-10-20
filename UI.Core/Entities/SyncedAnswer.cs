using Quibble.Core.Entities;
using Quibble.Core.Events;
using System;
using System.Threading.Tasks;

namespace Quibble.UI.Core.Entities
{
    public class SyncedAnswer : IParticipantAnswer, IDisposable
    {
        private Guid Token { get; } = Guid.NewGuid();

        public Guid Id { get; }
        public Guid QuestionId { get; }
        public Guid ParticipantId { get; }

        public string Text { get; set; }
        public AnswerMark Mark { get; private set; }

        private readonly AsyncEvent _updated = new();
        public event Func<Task> Updated
        {
            add => _updated.Add(value);
            remove => _updated.Remove(value);
        }

        private readonly SyncServices _services;
        private bool _isDisposed;

        internal SyncedAnswer(IParticipantAnswer dtoAnswer, SyncServices services)
        {
            _services = services;

            Id = dtoAnswer.Id;
            QuestionId = dtoAnswer.QuestionId;
            ParticipantId = dtoAnswer.ParticipantId;
            Mark = dtoAnswer.Mark;
            Text = dtoAnswer.Text;

            _services.AnswerEvents.AnswerMarkUpdated += OnAnswerMarkUpdatedAsync;
            _services.AnswerEvents.AnswerTextUpdated += OnAnswerTextUpdatedAsync;
        }

        public Task UpdateMarkAsync(AnswerMark newMark) => _services.AnswerService.UpdateMarkAsync(Id, newMark);
        public Task SaveTextAsync() => _services.AnswerService.UpdateTextAsync(Id, Text, Token);

        private Task OnAnswerMarkUpdatedAsync(Guid answerId, AnswerMark newMark)
        {
            if (answerId != Id) return Task.CompletedTask;

            Mark = newMark;

            return _updated.InvokeAsync();
        }

        private Task OnAnswerTextUpdatedAsync(Guid answerId, string newText, Guid initiatorToken)
        {
            if (Token == initiatorToken) return Task.CompletedTask;
            if (answerId != Id) return Task.CompletedTask;

            Text = newText;

            return _updated.InvokeAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _services.AnswerEvents.AnswerMarkUpdated -= OnAnswerMarkUpdatedAsync;
                    _services.AnswerEvents.AnswerTextUpdated -= OnAnswerTextUpdatedAsync;
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
