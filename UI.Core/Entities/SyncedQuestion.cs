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

        public QuestionState State { get; private set; }

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
            State = dtoQuestion.State;

            _services.QuestionEvents.AnswerUpdated += OnQuestionAnswerUpdatedAsync;
            _services.QuestionEvents.TextUpdated += OnQuestionTextUpdatedAsync;
            _services.QuestionEvents.StateUpdated += OnQuestionStateUpdatedAsync;
        }

        public Task SaveTextAsync() => _services.QuestionService.UpdateTextAsync(Id, QuestionText);

        public Task SaveAnswerAsync() => _services.QuestionService.UpdateAnswerAsync(Id, CorrectAnswer);

        public Task UpdateStateAsync(QuestionState newState) => _services.QuestionService.UpdateStateAsync(Id, newState);

        public Task DeleteAsync() =>_services.QuestionService.DeleteAsync(Id);

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

        private Task OnQuestionStateUpdatedAsync(Guid questionId, QuestionState newState)
        {
            if (questionId != Id) return Task.CompletedTask;

            State = newState;

            return _updated.InvokeAsync();
        }

        public void Dispose()
        {
            _services.QuestionEvents.AnswerUpdated -= OnQuestionAnswerUpdatedAsync;
            _services.QuestionEvents.TextUpdated -= OnQuestionTextUpdatedAsync;
            _services.QuestionEvents.StateUpdated -= OnQuestionStateUpdatedAsync;
        }
    }
}
