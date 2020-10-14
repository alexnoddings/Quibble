using System.Reflection;

namespace Quibble.UI.Core.Services
{
    public interface IAppMetadata
    {
        public HostModel HostingModel { get; }

        public Assembly[] GetAdditionalUIAssemblies();
    }
}