﻿using Microsoft.AspNetCore.Components;
using Quibble.Client.Services.Themeing;

namespace Quibble.Client.Components.Authentication
{
    public partial class ThemeDisplay
    {
        [Inject]
        private ThemeService ThemeService { get; set; } = default!;
    }
}
