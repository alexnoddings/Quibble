using System.Reflection;

namespace Quibble.UI.Core.Services
{
    public interface IAppMetadata
    {
        public string HostingModel { get; }

        public Assembly[] GetAdditionalUIAssemblies();
    }
}