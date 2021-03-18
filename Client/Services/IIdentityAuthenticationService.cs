using System.Threading.Tasks;
using Quibble.Shared.Authentication;

namespace Quibble.Client.Services
{
    public interface IIdentityAuthenticationService
    {
        Task<UserInfo> GetUserAsync();
        Task<AuthenticationOperation<UserInfo>> RegisterAsync(string username, string email, string password);
        Task<AuthenticationOperation<UserInfo>> LoginAsync(string username, string password, bool shouldRememberUser);
        Task LogoutAsync();
        Task<AuthenticationOperation> ForgotPasswordAsync(string email);
        Task<AuthenticationOperation> ResetPasswordAsync(string email, string token, string newPassword);
        Task<AuthenticationOperation> ChangePasswordAsync(string currentPassword, string newPassword);
        Task<AuthenticationOperation> RequestChangeEmailAsync(string currentPassword, string newEmail);
        Task<AuthenticationOperation> ChangeEmailAsync(string newEmail, string token);
        Task<AuthenticationOperation> ChangeUsernameAsync(string currentPassword, string newUsername);
    }
}
