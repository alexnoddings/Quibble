using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;

namespace Quibble.Client.Components
{
    public partial class DebugDisplay
    {
        [Inject]
        private IWebAssemblyHostEnvironment HostEnvironment { get; set; } = default!;

        [Inject]
        private IConfiguration Configuration { get; set; } = default!;
    }
}
