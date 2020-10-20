using Quibble.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Quibble.Host.Hosted.Platform.Events
{
    public interface IAnswerEventsInvoker
    {
        public Task InvokeNewAnswerSubmittedAsync(IParticipantAnswer answer);
        public Task InvokeAnswerTextUpdatedAsync(Guid id, string newText, Guid initiatorToken);
        public Task InvokeAnswerMarkUpdatedAsync(Guid id, AnswerMark newMark);
    }
}
