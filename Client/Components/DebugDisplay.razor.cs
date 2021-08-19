using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Quibble.Client.Components
{
    public partial class DebugDisplay
    {
        [Inject]
        private IWebAssemblyHostEnvironment HostEnvironment { get; set; } = default!;
    }
}
