using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorIdentityBase.Shared.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorIdentityBase.Client.Services
{
    public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IdentityAuthenticationService _authenticationService;
        private UserInfo _userInfo;

        public IdentityAuthenticationStateProvider(IdentityAuthenticationService authenticationService)
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
        public async Task<AuthenticationOperation> LoginAsync(string username, string password, bool shouldRememberUser)
        {
            var operation = await _authenticationService.LoginAsync(username, password, shouldRememberUser);
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

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            _userInfo ??= await _authenticationService.GetUserAsync();

            var identity = new ClaimsIdentity();
            if (_userInfo.IsAuthenticated)
                identity = new ClaimsIdentity(_userInfo.Claims.Select(kv => new Claim(kv.Key, kv.Value)), "Server Identity");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}
