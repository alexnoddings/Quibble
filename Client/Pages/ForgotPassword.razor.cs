﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Services.Authentication;
using Quibble.Shared.Api.Authentication;

namespace Quibble.Client.Pages
{
    public partial class ForgotPassword
    {
        [Inject]
        private IdentityAuthenticationStateProvider AuthenticationProvider { get; set; } = default!;

        private class ForgotPasswordModel : ForgotPasswordRequest
        {
        }

        private ForgotPasswordModel Model { get; } = new();

        private List<string>? Errors { get; set; }

        private bool WasSuccessful { get; set; } = false;

        private async Task ForgotPasswordAsync()
        {
            var result = await AuthenticationProvider.ForgotPasswordAsync(Model.Email);
            if (result.WasSuccessful)
                WasSuccessful = true;
            else
                Errors = result.Errors?.ToList() ?? new List<string>();
        }
    }
}
