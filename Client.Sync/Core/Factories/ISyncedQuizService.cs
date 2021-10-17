using Quibble.Client.Sync.Contexts;
using Quibble.Shared.Api;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Core
{
    public interface ISyncedQuizService
    {
        public Task<ApiResponse<(FullQuizDto, ISyncContext)>> GetQuizAsync(Guid id);
    }
}
