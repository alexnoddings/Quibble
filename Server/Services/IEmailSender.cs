using System.Threading.Tasks;

namespace Quibble.Server.Services
{
    public interface IEmailSender
    {
        public Task SendAsync(string address, string content);
    }
}
