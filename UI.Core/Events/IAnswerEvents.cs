using Quibble.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Quibble.UI.Core.Events
{
    public interface IAnswerEvents
    {
        public event Func<IParticipantAnswer, Task> NewAnswerSubmitted;
        public event Func<Guid, string, Guid, Task> AnswerTextUpdated;
        public event Func<Guid, AnswerMark, Task> AnswerMarkUpdated;
    }
}
