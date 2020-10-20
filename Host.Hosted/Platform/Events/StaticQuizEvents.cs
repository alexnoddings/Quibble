using System;
using System.Threading.Tasks;
using Quibble.Core.Events;
using Quibble.UI.Core.Events;

namespace Quibble.Host.Hosted.Platform.Events
{
    internal class StaticQuizEvents : IQuizEvents, IQuizEventsInvoker
    {
        public Task InvokeTitleUpdatedAsync(Guid id, string newTitle, Guid initiatorToken) => _titleUpdated.InvokeAsync(id, newTitle, initiatorToken);
        private static readonly AsyncEvent<Guid, string, Guid> _titleUpdated = new();
        public event Func<Guid, string, Guid, Task> TitleUpdated
        {
            add => _titleUpdated.Add(value);
            remove => _titleUpdated.Remove(value);
        }

        public Task InvokePublishedAsync(Guid id, DateTime time) => _published.InvokeAsync(id, time);
        private static readonly AsyncEvent<Guid, DateTime> _published = new();
        public event Func<Guid, DateTime, Task> Published
        {
            add => _published.Add(value);
            remove => _published.Remove(value);
        }

        public Task InvokeDeletedAsync(Guid id) => _deleted.InvokeAsync(id);
        private static readonly AsyncEvent<Guid> _deleted = new();
        public event Func<Guid, Task> Deleted
        {
            add => _deleted.Add(value);
            remove => _deleted.Remove(value);
        }
    }
}
