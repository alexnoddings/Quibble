using Quibble.Common.Authentication;
using System.Net.Http.Json;
using System.Text.Json;

namespace Quibble.Client.App.Services.Authentication;

public class IdentityAuthenticationService : IIdentityAuthenticationService
{
	private static readonly JsonSerializerOptions DefaultDeserialisationOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	private ILogger<IdentityAuthenticationService> Logger { get; }
	private HttpClient HttpClient { get; }

	public IdentityAuthenticationService(ILogger<IdentityAuthenticationService> logger, IHttpClientFactory httpClientFactory)
	{
		Logger = logger;
		HttpClient = httpClientFactory.CreateClient(nameof(IdentityAuthenticationService));
	}

	public async Task<UserInfo> GetUserAsync()
	{
		Logger.LogDebug("Fetching UserInfo.");

		var result = await HttpClient.GetAsync("User");
		if (!result.IsSuccessStatusCode)
		{
			Logger.LogWarning("User endpoint returned an unsuccessful response: {StatusCode}.", result.StatusCode);
			return UserInfo.Unauthenticated();
		}

		using var contentStream = await result.Content.ReadAsStreamAsync();

		UserInfo? userInfo;
		try
		{
			userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(contentStream, DefaultDeserialisationOptions);
		}
		catch (JsonException ex)
		{
			Logger.LogError("Failed to deserialise UserInfo: {Exception}.", ex);
			return UserInfo.Unauthenticated();
		}

		if (userInfo is null)
		{
			Logger.LogWarning("User endpoint did not return a result.");
			return UserInfo.Unauthenticated();
		}

		if (userInfo.IsAuthenticated)
			Logger.LogInformation("Logged in as {UserName}.", userInfo.UserName);
		else
			Logger.LogInformation("User is not logged in.");

		return userInfo ?? UserInfo.Unauthenticated();

	}

	public Task<AuthenticationOperation<UserInfo>> RegisterAsync(string username, string email, string password) =>
		PerformPostAsync<RegisterRequest, UserInfo>("Register", new RegisterRequest { UserName = username, Email = email, Password = password });

	public Task<AuthenticationOperation<UserInfo>> LoginAsync(string username, string password) =>
		PerformPostAsync<LoginRequest, UserInfo>("Login", new LoginRequest { UserName = username, Password = password });

	public Task LogoutAsync() =>
		HttpClient.PostAsync("Logout", null);

	public Task<AuthenticationOperation> ForgotPasswordAsync(string email) =>
		PerformPostAsync("ForgotPassword", new ForgotPasswordRequest { Email = email });

	public Task<AuthenticationOperation> ResetPasswordAsync(string email, string token, string newPassword) =>
		PerformPostAsync("ResetPassword", new ResetPasswordRequest { Email = email, Token = token, NewPassword = newPassword });

	public Task<AuthenticationOperation> ChangePasswordAsync(string currentPassword, string newPassword) =>
		PerformPostAsync("ChangePassword", new ChangePasswordRequest { CurrentPassword = currentPassword, NewPassword = newPassword });

	public Task<AuthenticationOperation> RequestChangeEmailAsync(string currentPassword, string newEmail) =>
		PerformPostAsync("RequestChangeEmail", new RequestChangeEmailRequest { Password = currentPassword, NewEmail = newEmail });

	public Task<AuthenticationOperation> ChangeEmailAsync(string newEmail, string token) =>
		PerformPostAsync("ChangeEmail", new ChangeEmailRequest { NewEmail = newEmail, Token = token });

	public Task<AuthenticationOperation> ChangeUsernameAsync(string currentPassword, string newUsername) =>
		PerformPostAsync("ChangeUsername", new ChangeUsernameRequest { Password = currentPassword, NewUsername = newUsername });

	private async Task<AuthenticationOperation> PerformPostAsync<TValue>(string endpoint, TValue value)
	{
		var result = await HttpClient.PostAsJsonAsync(endpoint, value);

		if (result.IsSuccessStatusCode)
			return AuthenticationOperation.FromSuccess();

		var contentStream = await result.Content.ReadAsStreamAsync();
		var errors = await JsonSerializer.DeserializeAsync<List<string>>(contentStream, DefaultDeserialisationOptions);
		return AuthenticationOperation.FromError(errors ?? new List<string> { "An unknown error has occurred." });
	}

	private async Task<AuthenticationOperation<TReturn>> PerformPostAsync<TValue, TReturn>(string endpoint, TValue value)
	{
		var result = await HttpClient.PostAsJsonAsync(endpoint, value);
		var contentStream = await result.Content.ReadAsStreamAsync();

		if (result.IsSuccessStatusCode)
		{
			var returnValue = await JsonSerializer.DeserializeAsync<TReturn>(contentStream, DefaultDeserialisationOptions);
			return AuthenticationOperation<TReturn>.FromSuccess(returnValue);
		}

		var errors = await JsonSerializer.DeserializeAsync<List<string>>(contentStream, DefaultDeserialisationOptions);
		return AuthenticationOperation<TReturn>.FromError(errors ?? new List<string> { "An unknown error occurred." });
	}
}
