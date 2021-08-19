using Quibble.Client.Sync.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync
{
    public interface ISynchronisedQuizFactory
    {
        Task<HubResponse<ISynchronisedQuiz>> GetQuizAsync(Guid quizId);
    }
}
