using System.Reflection;
using Quibble.UI.Core.Services;

namespace Quibble.Host.WASM.Client.Platform
{
    public class WasmAppMetadata : IAppMetadata
    {
        public HostModel HostingModel => HostModel.WASM;

        public Assembly[] GetAdditionalUIAssemblies() => new[] { typeof(WasmClientStartup).Assembly };
    }
}