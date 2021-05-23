using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Quibble.Server.Services.EmailSender
{
    public class DebugToLogEmailSender : IEmailSender
    {
        private readonly ILogger<DebugToLogEmailSender> _logger;

        public DebugToLogEmailSender(ILogger<DebugToLogEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string address, string content)
        {
            _logger.LogInformation("{Address}: {Content}", address, content);
            return Task.CompletedTask;
        }
    }
}
