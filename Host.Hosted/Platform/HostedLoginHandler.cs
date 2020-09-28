using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Services;

namespace Quibble.Host.Hosted.Platform
{
    public class HostedLoginHandler : ILoginHandler
    {
        private NavigationManager NavigationManager { get; }

        public HostedLoginHandler(NavigationManager navigationManager)
        {
            NavigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        }

        public string ProfileUrl => "Identity/Account/Manage";

        // Blazor Server requires a local ReturnUrl (e.g. /example)
        public string RegisterUrl => $"Identity/Account/Register?ReturnUrl={CurrentPath}";
        public string LoginUrl => $"Identity/Account/Login?ReturnUrl={CurrentPath}";
        private string CurrentPath => new Uri(NavigationManager.Uri).AbsolutePath;

        public Task BeginSignOutAsync()
        {
            NavigationManager.NavigateTo("Identity/Account/LogOut", forceLoad: true);
            return Task.CompletedTask;
        }
    }
}