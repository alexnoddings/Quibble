using Quibble.Client.Core.Entities;
using Quibble.Common.Api;

namespace Quibble.Client.Core.Factories;

public interface ISyncedQuizFactory
{
	public Task<ApiResponse<ISyncedQuiz>> GetSyncedQuizAsync(Guid id);
}
