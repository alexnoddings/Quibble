using System.Threading.Tasks;
using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Entities.HostMode
{
    public interface ISynchronisedHostModeSubmittedAnswer : ISubmittedAnswer, ISynchronisedEntity
    {
        public ISynchronisedHostModeQuestion Question { get; }
        public ISynchronisedHostModeParticipant Submitter { get; }

        public Task MarkAsync(double points);
    }
}
