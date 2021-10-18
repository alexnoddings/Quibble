using Bunit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Quibble.Client.Extensions;

internal static class EnvironmentExtensions
{
    public static TestContext AddEnvironment(this TestContext context, string environment = "Production")
    {
        var webAssemblyHostEnvironmentMock = new Mock<IWebAssemblyHostEnvironment>(MockBehavior.Loose);
        webAssemblyHostEnvironmentMock.SetupGet(hostEnvironment => hostEnvironment.Environment).Returns(environment);

        context.Services.AddSingleton(webAssemblyHostEnvironmentMock.Object);

        return context;
    }
}
