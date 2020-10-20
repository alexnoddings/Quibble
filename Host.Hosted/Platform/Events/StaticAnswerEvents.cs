using Quibble.Core.Entities;
using Quibble.Core.Events;
using Quibble.UI.Core.Events;
using System;
using System.Threading.Tasks;

namespace Quibble.Host.Hosted.Platform.Events
{
    public class StaticAnswerEvents : IAnswerEvents, IAnswerEventsInvoker
    {
        public Task InvokeNewAnswerSubmittedAsync(IParticipantAnswer answer) => _newAnswerSubmitted.InvokeAsync(answer);
        private static readonly AsyncEvent<IParticipantAnswer> _newAnswerSubmitted = new();
        public event Func<IParticipantAnswer, Task> NewAnswerSubmitted
        {
            add => _newAnswerSubmitted.Add(value);
            remove => _newAnswerSubmitted.Remove(value);
        }

        public Task InvokeAnswerTextUpdatedAsync(Guid id, string newText, Guid initiatorToken) => _answerTextUpdated.InvokeAsync(id, newText, initiatorToken);
        private static readonly AsyncEvent<Guid, string, Guid> _answerTextUpdated = new();
        public event Func<Guid, string, Guid, Task> AnswerTextUpdated
        {
            add => _answerTextUpdated.Add(value);
            remove => _answerTextUpdated.Remove(value);
        }

        public Task InvokeAnswerMarkUpdatedAsync(Guid id, AnswerMark newMark) => _answerMarkUpdated.InvokeAsync(id, newMark);
        private static readonly AsyncEvent<Guid, AnswerMark> _answerMarkUpdated = new();
        public event Func<Guid, AnswerMark, Task> AnswerMarkUpdated
        {
            add => _answerMarkUpdated.Add(value);
            remove => _answerMarkUpdated.Remove(value);
        }
    }
}
