using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;

namespace Quibble.UI.Core.Entities
{
    public sealed class SyncedQuestion : IQuestion, IDisposable
    {
        public Guid Id { get; set; }
        public Guid RoundId { get; set; }
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }

        private readonly AsyncEvent _updated = new();
        public event Func<Task> Updated
        {
            add => _updated.Add(value);
            remove => _updated.Remove(value);
        }

        private readonly SyncServices _services;

        internal SyncedQuestion(IQuestion dtoQuestion, SyncServices services)
        {
            _services = services;

            Id = dtoQuestion.Id;
            RoundId = dtoQuestion.RoundId;
            QuestionText = dtoQuestion.QuestionText;
            CorrectAnswer = dtoQuestion.CorrectAnswer;

            _services.QuestionEvents.AnswerUpdated += OnQuestionAnswerUpdatedAsync;
            _services.QuestionEvents.TextUpdated += OnQuestionTextUpdatedAsync;
        }

        public Task SaveTextAsync()
        {
            return _services.QuestionService.UpdateTextAsync(Id, QuestionText);
        }

        public Task SaveAnswerAsync()
        {
            return _services.QuestionService.UpdateAnswerAsync(Id, CorrectAnswer);
        }

        public Task DeleteAsync()

        {
            return _services.QuestionService.DeleteAsync(Id);
        }

        private Task OnQuestionTextUpdatedAsync(Guid questionId, string newText)
        {
            if (questionId != Id) return Task.CompletedTask;

            QuestionText = newText;

            return _updated.InvokeAsync();
        }

        private Task OnQuestionAnswerUpdatedAsync(Guid questionId, string newAnswer)
        {
            if (questionId != Id) return Task.CompletedTask;

            CorrectAnswer = newAnswer;

            return _updated.InvokeAsync();
        }

        public void Dispose()
        {
            _services.QuestionEvents.AnswerUpdated -= OnQuestionAnswerUpdatedAsync;
            _services.QuestionEvents.TextUpdated -= OnQuestionTextUpdatedAsync;
        }
    }
}
