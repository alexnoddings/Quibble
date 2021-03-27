using System;
using System.Threading.Tasks;

namespace Quibble.Shared.Hubs
{
    public interface IEventHub
    {
        public Task JoinAsync(Guid quizId);
        public Task LeaveAsync(Guid quizId);
    }
}
