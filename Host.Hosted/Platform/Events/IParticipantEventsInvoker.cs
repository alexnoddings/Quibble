using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;

namespace Quibble.Host.Hosted.Platform.Events
{
    public interface IParticipantEventsInvoker
    {
        public Task InvokeParticipantJoinedAsync(IParticipant participant);
        public Task InvokeParticipantLeftAsync(Guid participantId);
    }
}
