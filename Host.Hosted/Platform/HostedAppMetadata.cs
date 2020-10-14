using System;
using System.Reflection;
using Quibble.UI.Core.Services;

namespace Quibble.Host.Hosted.Platform
{
    public class HostedAppMetadata : IAppMetadata
    {
        public HostModel HostingModel => HostModel.Server;

        public Assembly[] GetAdditionalUIAssemblies() => Array.Empty<Assembly>();
    }
}