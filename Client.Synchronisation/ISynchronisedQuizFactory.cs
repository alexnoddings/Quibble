using System;
using System.Threading.Tasks;
using Quibble.Client.Sync.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync
{
    public interface ISynchronisedQuizFactory
    {
        Task<HubResponse<ISynchronisedEntity>> GetQuizAsync(Guid quizId);
    }
}
