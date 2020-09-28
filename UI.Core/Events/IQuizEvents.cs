using System;
using System.Threading.Tasks;

namespace Quibble.UI.Core.Events
{
    public interface IQuizEvents
    {
        public event Func<Guid, string, Task> TitleUpdated;
        public event Func<Guid, DateTime, Task> Published;
        public event Func<Guid, Task> Deleted;
    }
}
