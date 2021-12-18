using Quibble.Client.Core.Contexts;
using Quibble.Common.Api;
using Quibble.Common.Dtos;

namespace Quibble.Client.Core.Factories;

public interface ISyncedQuizService
{
	public Task<ApiResponse<(FullQuizDto, ISyncContext)>> GetQuizAsync(Guid id);
}
