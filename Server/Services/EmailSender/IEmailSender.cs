namespace Quibble.Server.Services.EmailSender;

public interface IEmailSender
{
	public Task SendAsync(string address, string content);
}
