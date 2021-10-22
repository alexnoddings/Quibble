using Quibble.Client.Sync.Core.Entities;
using Quibble.Shared.Api;

namespace Quibble.Client.Sync.Core.Factories;

public interface ISyncedQuizFactory
{
    public Task<ApiResponse<ISyncedQuiz>> GetSyncedQuizAsync(Guid id);
}
