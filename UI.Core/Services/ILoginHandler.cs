using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Quibble.UI.Core.Services
{
    [SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Strings are much easier to work with for Razor UIs")]
    public interface ILoginHandler
    {
        public string ProfileUrl { get; }
        public string RegisterUrl { get; }
        public string LoginUrl { get; }

        public Task BeginSignOutAsync();
    }
}