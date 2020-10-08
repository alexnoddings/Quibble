using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;
using Quibble.UI.Core.Events;

namespace Quibble.Host.Hosted.Platform.Events
{
    internal class StaticParticipantEvents : IParticipantEvents, IParticipantEventsInvoker
    {
        public Task InvokeParticipantJoinedAsync(IParticipant participant) => _participantJoined.InvokeAsync(participant);
        private static readonly AsyncEvent<IParticipant> _participantJoined = new();
        public event Func<IParticipant, Task> ParticipantJoined
        {
            add => _participantJoined.Add(value);
            remove => _participantJoined.Remove(value);
        }

        public Task InvokeParticipantLeftAsync(Guid participantId) => _participantLeft.InvokeAsync(participantId);
        private static readonly AsyncEvent<Guid> _participantLeft = new();
        public event Func<Guid, Task> ParticipantLeft
        {
            add => _participantLeft.Add(value);
            remove => _participantLeft.Remove(value);
        }
    }
}
