using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;

namespace Quibble.UI.Core.Events
{
    public interface IRoundEvents
    {
        public event Func<Guid, string, Guid, Task> TitleUpdated;
        public event Func<Guid, RoundState, Task> StateUpdated;
        public event Func<IRound, Task> RoundAdded;
        public event Func<Guid, Task> RoundDeleted;
    }
}
