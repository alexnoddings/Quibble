using Quibble.Common.Dtos;
using System.Net.Http.Json;
using System.Text.Json;

namespace Quibble.Client.App.Services.Quiz;

public class HttpQuizService : IQuizService
{
	private ILogger<HttpQuizService> Logger { get; }
	private HttpClient HttpClient { get; }

	public HttpQuizService(ILogger<HttpQuizService> logger, IHttpClientFactory HttpClientFactory)
	{
		Logger = logger;
		HttpClient = HttpClientFactory.CreateClient("QuizApi");
	}

	public Task<UserQuizzesDto?> GetUserQuizzesAsync() =>
		TryGetFromJsonAsync<UserQuizzesDto>("");

	public Task<SiteStatsDto?> GetSiteStatsAsync() =>
		TryGetFromJsonAsync<SiteStatsDto>("stats");

	private async Task<T?> TryGetFromJsonAsync<T>(string requestUri)
	{
		Logger.LogDebug("Loading {Type} from {RequestUri}.", typeof(T).Name, requestUri);
		try
		{
			return await HttpClient.GetFromJsonAsync<T>(requestUri);
		}
		catch (JsonException ex)
		{
			Logger.LogError("Failed to load {Type}: {Exception}", typeof(T).Name, ex);
			return default;
		}
	}

	public async Task<Guid> CreateNewQuizAsync()
	{
		var response = await HttpClient.PostAsync("", null);
		response.EnsureSuccessStatusCode();

		var result = await response.Content.ReadFromJsonAsync<Guid?>();
		if (!result.HasValue)
			throw new InvalidOperationException();

		return result.Value;
	}
}

