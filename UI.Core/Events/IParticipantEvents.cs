using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;

namespace Quibble.UI.Core.Events
{
    public interface IParticipantEvents
    {
        public event Func<IParticipant, Task> ParticipantJoined;
        public event Func<Guid, Task> ParticipantLeft;
    }
}
