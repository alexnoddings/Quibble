using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;

namespace Quibble.Host.Hosted.Platform.Events
{
    public interface IQuestionEventsInvoker
    {
        public Task InvokeTextUpdatedAsync(Guid id, string newText, Guid initiatorToken);
        public Task InvokeCorrectAnswerUpdatedAsync(Guid id, string newAnswer, Guid initiatorToken);
        public Task InvokeStateUpdatedAsync(Guid id, QuestionState newState);
        public Task InvokeQuestionAddedAsync(IQuestion newQuestion);
        public Task InvokeQuestionDeletedAsync(Guid id);
    }
}