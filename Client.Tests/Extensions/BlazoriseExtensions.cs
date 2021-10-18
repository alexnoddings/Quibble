using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Bunit;

namespace Quibble.Client.Extensions;

internal static class BlazoriseExtensions
{
    public static TestContext AddBlazorise(this TestContext context)
    {
        context.Services
            .AddBlazorise(options =>
            {
                options.ChangeTextOnKeyPress = true;
                options.DelayTextOnKeyPress = true;
                options.DelayTextOnKeyPressInterval = 80;
            })
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

        context.JSInterop.SetupVoid("blazorise.unregisterClosableComponent", _ => true);

        return context;
    }
}

