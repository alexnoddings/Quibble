using System.Reflection;
using Quibble.UI.Core.Services;

namespace Quibble.Host.WASM.Client.Platform
{
    public class WasmAppMetadata : IAppMetadata
    {
        public string HostingModel => "WASM Hosted";

        public Assembly[] GetAdditionalUIAssemblies() => new[] { typeof(Startup).Assembly };
    }
}