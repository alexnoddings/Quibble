using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;
using Quibble.UI.Core.Events;

namespace Quibble.Host.Hosted.Platform.Events
{
    internal class StaticQuestionEvents : IQuestionEvents, IQuestionEventsInvoker
    {
        public Task InvokeTextUpdatedAsync(Guid id, string newText, Guid initiatorToken) => _textUpdated.InvokeAsync(id, newText, initiatorToken);
        private static readonly AsyncEvent<Guid, string, Guid> _textUpdated = new();
        public event Func<Guid, string, Guid, Task> TextUpdated
        {
            add => _textUpdated.Add(value);
            remove => _textUpdated.Remove(value);
        }

        public Task InvokeCorrectAnswerUpdatedAsync(Guid id, string newAnswer, Guid initiatorToken) => _correctAnswerUpdated.InvokeAsync(id, newAnswer, initiatorToken);
        private static readonly AsyncEvent<Guid, string, Guid> _correctAnswerUpdated = new();
        public event Func<Guid, string, Guid, Task> CorrectAnswerUpdated
        {
            add => _correctAnswerUpdated.Add(value);
            remove => _correctAnswerUpdated.Remove(value);
        }

        public Task InvokeStateUpdatedAsync(Guid id, QuestionState newState) => _stateUpdated.InvokeAsync(id, newState);
        private static readonly AsyncEvent<Guid, QuestionState> _stateUpdated = new();
        public event Func<Guid, QuestionState, Task> StateUpdated
        {
            add => _stateUpdated.Add(value);
            remove => _stateUpdated.Remove(value);
        }

        public Task InvokeQuestionAddedAsync(IQuestion newQuestion) => _questionAdded.InvokeAsync(newQuestion);
        private static readonly AsyncEvent<IQuestion> _questionAdded = new();
        public event Func<IQuestion, Task> QuestionAdded
        {
            add => _questionAdded.Add(value);
            remove => _questionAdded.Remove(value);
        }

        public Task InvokeQuestionDeletedAsync(Guid id) => _questionDeleted.InvokeAsync(id);
        private static readonly AsyncEvent<Guid> _questionDeleted = new();
        public event Func<Guid, Task> QuestionDeleted
        {
            add => _questionDeleted.Add(value);
            remove => _questionDeleted.Remove(value);
        }
    }
}
