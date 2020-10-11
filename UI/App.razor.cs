using Blazorise;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Services;
using Quibble.UI.Core.Services.Theme;

namespace Quibble.UI
{
    public sealed partial class App
    {
        [Inject]
        private IThemeProvider ThemeProvider { get; init; } = default!;

        [Inject]
        private ILoginHandler LoginHandler { get; init; } = default!;

        [Inject]
        private IAppMetadata AppMetadata { get; init; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; init; } = default!;

        private Theme Theme =>
            new Theme
            {
                ColorOptions = new ThemeColorOptions
                {
                    Primary = ThemeProvider.Primary.Colour,
                    Secondary = ThemeProvider.Secondary.Colour,
                    Success = ThemeProvider.Success.Colour,
                    Info = ThemeProvider.Info.Colour,
                    Warning = ThemeProvider.Warning.Colour,
                    Danger = ThemeProvider.Danger.Colour,
                    Light = ThemeProvider.Light.Colour,
                    Dark = ThemeProvider.Dark.Colour,
                },
                TextColorOptions = new ThemeTextColorOptions
                {
                    Primary = ThemeProvider.Primary.Text,
                    Secondary = ThemeProvider.Secondary.Text,
                    Success = ThemeProvider.Success.Text,
                    Info = ThemeProvider.Info.Text,
                    Warning = ThemeProvider.Warning.Text,
                    Danger = ThemeProvider.Danger.Text,
                    Light = ThemeProvider.Light.Text,
                    Dark = ThemeProvider.Dark.Text,
                    Body = ThemeProvider.Body,
                    Muted = ThemeProvider.Muted,
                    White = ThemeProvider.White,
                    Black50 = ThemeProvider.Black50,
                    White50 = ThemeProvider.White50,
                },
                LuminanceThreshold = ThemeProvider.LuminanceThreshold,
            };
    }
}
