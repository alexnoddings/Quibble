﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Services.Authentication;
using Quibble.Shared.Api.Authentication;

namespace Quibble.Client.Pages.Settings
{
    public partial class ChangeEmail
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

        private class RequestChangeEmailModel : RequestChangeEmailRequest
        {
        }

        private RequestChangeEmailModel Model { get; } = new();

        private List<string>? Errors { get; set; }

        private bool WasSuccessful { get; set; }

        private string? CurrentEmail { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CurrentEmail = (await AuthenticationProvider.GetAuthenticationStateAsync()).User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
        }

        private async Task RequestChangeEmailAsync()
        {
            WasSuccessful = false;
            var result = await AuthenticationProvider.RequestChangeEmailAsync(Model.Password, Model.NewEmail);
            WasSuccessful = result.WasSuccessful;
            if (WasSuccessful)
                Errors = null;
            else
                Errors = result.Errors?.ToList() ?? new List<string>();
        }
    }
}
