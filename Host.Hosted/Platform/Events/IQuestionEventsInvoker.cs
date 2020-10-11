using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;

namespace Quibble.Host.Hosted.Platform.Events
{
    public interface IQuestionEventsInvoker
    {
        public Task InvokeTextUpdatedAsync(Guid id, string newText);
        public Task InvokeAnswerUpdatedAsync(Guid id, string newAnswer);
        public Task InvokeStateUpdatedAsync(Guid id, QuestionState newState);
        public Task InvokeQuestionAddedAsync(IQuestion newQuestion);
        public Task InvokeQuestionDeletedAsync(Guid id);
    }
}