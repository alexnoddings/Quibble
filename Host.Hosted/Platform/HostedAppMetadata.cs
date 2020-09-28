using System;
using System.Reflection;
using Quibble.UI.Core.Services;

namespace Quibble.Host.Hosted.Platform
{
    public class HostedAppMetadata : IAppMetadata
    {
        public string HostingModel => "Server Hosted";

        public Assembly[] GetAdditionalUIAssemblies() => Array.Empty<Assembly>();
    }
}