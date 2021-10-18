using Quibble.Client.Sync.Core;
using Quibble.Shared.Api;

namespace Quibble.Client.Sync
{
    public interface ISyncedQuizFactory
    {
        public Task<ApiResponse<ISyncedQuiz>> GetSyncedQuizAsync(Guid id);
    }
}
