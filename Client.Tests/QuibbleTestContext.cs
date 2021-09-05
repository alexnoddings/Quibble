using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Bunit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Quibble.Client
{
    public class QuibbleTestContext : TestContext
    {
        protected void AddBlazorise()
        {
            Services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                    options.DelayTextOnKeyPress = true;
                    options.DelayTextOnKeyPressInterval = 80;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            JSInterop.SetupVoid("blazorise.unregisterClosableComponent", _ => true);
        }

        protected void AddEnvironment(string environment = "Production")
        {
            var webAssemblyHostEnvironmentMock = new Mock<IWebAssemblyHostEnvironment>(MockBehavior.Loose);
            webAssemblyHostEnvironmentMock.SetupGet(hostEnvironment => hostEnvironment.Environment).Returns(environment);

            Services.AddSingleton(webAssemblyHostEnvironmentMock.Object);
        }
    }
}
