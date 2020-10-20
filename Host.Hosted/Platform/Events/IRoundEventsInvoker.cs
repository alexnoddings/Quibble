using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;

namespace Quibble.Host.Hosted.Platform.Events
{
    public interface IRoundEventsInvoker
    {
        public Task InvokeTitleUpdatedAsync(Guid id, string newTitle, Guid initiatorToken);
        public Task InvokeStateUpdatedAsync(Guid id, RoundState newState);
        public Task InvokeRoundAddedAsync(IRound newRound);
        public Task InvokeRoundDeletedAsync(Guid id);
    }
}