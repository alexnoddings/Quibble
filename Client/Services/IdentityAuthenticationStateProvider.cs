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

        public async Task<AuthenticationOperation> LoginAsync(string username, string password, bool shouldRememberUser)
        {
            var operation = await _authenticationService.LoginAsync(username, password, shouldRememberUser);
            if (operation.WasSuccessful)
            {
                _userInfo = null;
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }

            // The operation is returned without the <TResult> as claims aren't populated in login/register requests.
            // These will be populated later by GetAuthenticationStateAsync.
            return operation;
        }

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

        public async Task LogoutAsync()
        {
            await _authenticationService.LogoutAsync();
            _userInfo = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

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
