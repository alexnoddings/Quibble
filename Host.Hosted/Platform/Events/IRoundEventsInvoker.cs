using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;

namespace Quibble.Host.Hosted.Platform.Events
{
    public interface IRoundEventsInvoker
    {
        public Task InvokeTitleUpdatedAsync(Guid id, string newTitle);
        public Task InvokeRoundAddedAsync(IRound newRound);
        public Task InvokeRoundDeletedAsync(Guid id);
    }
}