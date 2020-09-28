using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Quibble.UI.Core.Services;

namespace Quibble.Host.WASM.Client.Platform
{
    public class WasmLoginHandler : ILoginHandler
    {
        private NavigationManager NavigationManager { get; }
        private SignOutSessionStateManager SignOutManager { get; }

        public WasmLoginHandler(NavigationManager navigationManager, SignOutSessionStateManager signOutManager)
        {
            NavigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
            SignOutManager = signOutManager ?? throw new ArgumentNullException(nameof(signOutManager));
        }

        public string ProfileUrl => "authentication/profile";

        // Blazor WASM requires a full ReturnUrl (e.g. https://localhost/example)
        public string RegisterUrl => $"authentication/register?ReturnUrl={NavigationManager.Uri}";
        public string LoginUrl => $"authentication/login?ReturnUrl={NavigationManager.Uri}";

        public async Task BeginSignOutAsync()
        {
            await SignOutManager.SetSignOutState().ConfigureAwait(false);
            NavigationManager.NavigateTo("authentication/logout");
        }
    }
}