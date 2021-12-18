using Microsoft.AspNetCore.Components.Authorization;
using Quibble.Common.Authentication;
using System.Security.Claims;

namespace Quibble.Client.App.Services.Authentication;

public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
{
	private readonly IIdentityAuthenticationService _authenticationService;
	private UserInfo? _userInfo;

	public IdentityAuthenticationStateProvider(IIdentityAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	/// <remarks>
	/// The operation is returned without the &lt;TResult&gt; as claims aren't populated in login/register requests.
	///  These are populated by GetAuthenticationStateAsync.
	/// </remarks>
	public async Task<AuthenticationOperation> RegisterAsync(string username, string email, string password)
	{
		var operation = await _authenticationService.RegisterAsync(username, email, password);
		if (operation.WasSuccessful)
		{
			_userInfo = null;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		// The operation is returned without the <TResult> as claims aren't populated in login/register requests.
		// These will be populated later by GetAuthenticationStateAsync.
		return operation;
	}

	/// <remarks>
	/// The operation is returned without the &lt;TResult&gt; as claims aren't populated in login/register requests.
	///  These are populated by GetAuthenticationStateAsync.
	/// </remarks>
	public async Task<AuthenticationOperation> LoginAsync(string username, string password)
	{
		var operation = await _authenticationService.LoginAsync(username, password);
		if (operation.WasSuccessful)
		{
			_userInfo = null;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		return operation;
	}

	public async Task LogoutAsync()
	{
		await _authenticationService.LogoutAsync();
		_userInfo = null;
		NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
	}

	public Task<AuthenticationOperation> ForgotPasswordAsync(string email) =>
		_authenticationService.ForgotPasswordAsync(email);

	/// <remarks>
	/// The operation is returned without the &lt;TResult&gt; as claims aren't populated in login/register requests.
	///  These are populated by GetAuthenticationStateAsync.
	/// </remarks>
	public async Task<AuthenticationOperation> ResetPasswordAsync(string email, string token, string newPassword)
	{
		var operation = await _authenticationService.ResetPasswordAsync(email, token, newPassword);
		if (operation.WasSuccessful)
		{
			_userInfo = null;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		return operation;
	}

	public Task<AuthenticationOperation> ChangePasswordAsync(string currentPassword, string newPassword) =>
		_authenticationService.ChangePasswordAsync(currentPassword, newPassword);

	public Task<AuthenticationOperation> RequestChangeEmailAsync(string currentPassword, string newEmail) =>
		_authenticationService.RequestChangeEmailAsync(currentPassword, newEmail);

	public async Task<AuthenticationOperation> ChangeEmailAsync(string newEmail, string token)
	{
		var operation = await _authenticationService.ChangeEmailAsync(newEmail, token);
		if (operation.WasSuccessful)
		{
			_userInfo = null;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		return operation;
	}

	public async Task<AuthenticationOperation> ChangeUsernameAsync(string currentPassword, string newUsername)
	{
		var operation = await _authenticationService.ChangeUsernameAsync(currentPassword, newUsername);
		if (operation.WasSuccessful)
		{
			_userInfo = null;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		return operation;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		_userInfo ??= await _authenticationService.GetUserAsync();

		var identity = new ClaimsIdentity();
		if (_userInfo.IsAuthenticated)
			identity = new ClaimsIdentity(_userInfo.Claims.Select(kv => new Claim(kv.Key, kv.Value)), _userInfo.AuthenticationType);

		return new AuthenticationState(new ClaimsPrincipal(identity));
	}
}
