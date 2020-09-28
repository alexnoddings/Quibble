using System;
using System.Threading.Tasks;

namespace Quibble.Host.Hosted.Platform.Events
{
    public interface IQuizEventsInvoker
    {
        public Task InvokeTitleUpdatedAsync(Guid id, string newTitle);
        public Task InvokePublishedAsync(Guid id, DateTime time);
        public Task InvokeDeletedAsync(Guid id);
    }
}