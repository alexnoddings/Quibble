using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;
using Quibble.UI.Core.Events;

namespace Quibble.Host.Hosted.Platform.Events
{
    internal class StaticQuestionEvents : IQuestionEvents, IQuestionEventsInvoker
    {
        public Task InvokeTextUpdatedAsync(Guid id, string newText) => _textUpdated.InvokeAsync(id, newText);
        private static readonly AsyncEvent<Guid, string> _textUpdated = new();
        public event Func<Guid, string, Task> TextUpdated
        {
            add => _textUpdated.Add(value);
            remove => _textUpdated.Remove(value);
        }

        public Task InvokeAnswerUpdatedAsync(Guid id, string newAnswer) => _answerUpdated.InvokeAsync(id, newAnswer);
        private static readonly AsyncEvent<Guid, string> _answerUpdated = new();
        public event Func<Guid, string, Task> AnswerUpdated
        {
            add => _answerUpdated.Add(value);
            remove => _answerUpdated.Remove(value);
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
