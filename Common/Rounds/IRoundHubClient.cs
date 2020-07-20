using System;
using System.Threading.Tasks;

namespace Quibble.Common.Rounds
{
    public interface IRoundHubClient
    {
        public Task OnRoundCreated(Round round);

        public Task OnRoundUpdated(Round round);

        public Task OnRoundDeleted(Guid roundId);
    }
}
