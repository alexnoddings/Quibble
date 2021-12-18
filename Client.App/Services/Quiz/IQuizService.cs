using Quibble.Common.Dtos;

namespace Quibble.Client.App.Services.Quiz;

public interface IQuizService
{
	public Task<UserQuizzesDto?> GetUserQuizzesAsync();
	public Task<SiteStatsDto?> GetSiteStatsAsync();
	public Task<Guid> CreateNewQuizAsync();
}
