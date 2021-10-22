using Quibble.Client.Sync.Core.Contexts;
using Quibble.Shared.Api;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Core.Factories;

public interface ISyncedQuizService
{
    public Task<ApiResponse<(FullQuizDto, ISyncContext)>> GetQuizAsync(Guid id);
}
