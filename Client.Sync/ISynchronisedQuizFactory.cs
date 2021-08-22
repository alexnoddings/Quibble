using Quibble.Client.Sync.Entities;
using Quibble.Shared.Api;

namespace Quibble.Client.Sync
{
    public interface ISynchronisedQuizFactory
    {
        Task<ApiResponse<ISynchronisedQuiz>> GetQuizAsync(Guid quizId);
    }
}
