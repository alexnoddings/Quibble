using System.Threading.Tasks;

namespace BlazorIdentityBase.Server.Services
{
    public interface IEmailSender
    {
        public Task SendAsync(string address, string content);
    }
}
