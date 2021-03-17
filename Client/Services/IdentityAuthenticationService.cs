using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorIdentityBase.Shared.Authentication;

namespace BlazorIdentityBase.Client.Services
{
    public class IdentityAuthenticationService
    {
        private const string ApiBase = "/api/Authentication/";

        private static readonly JsonSerializerOptions DefaultDeserialisationOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public IdentityAuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthenticationOperation<UserInfo>> RegisterAsync(string username, string email, string password)
        {
            var request = new RegisterRequest { UserName = username, Email = email, Password = password };
            var result = await _httpClient.PostAsJsonAsync(ApiBase + "Register", request);
            var contentStream = await result.Content.ReadAsStreamAsync();

            if (result.IsSuccessStatusCode)
            {
                var userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(contentStream, DefaultDeserialisationOptions);
                return AuthenticationOperation<UserInfo>.FromSuccess(userInfo);
            }

            var errors = await JsonSerializer.DeserializeAsync<List<string>>(contentStream, DefaultDeserialisationOptions);
            return AuthenticationOperation<UserInfo>.FromError(errors);
        }

        public async Task<AuthenticationOperation<UserInfo>> LoginAsync(string username, string password, bool shouldRememberUser)
        {
            var request = new LoginRequest { UserName = username, Password = password, ShouldRememberUser = shouldRememberUser};
            var request = new LoginRequest { UserName = username, Password = password, ShouldRememberUser = shouldRememberUser };
            var result = await _httpClient.PostAsJsonAsync(ApiBase + "Login", request);
            var contentStream = await result.Content.ReadAsStreamAsync();

            if (result.IsSuccessStatusCode)
            {
                var userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(contentStream, DefaultDeserialisationOptions);
                return AuthenticationOperation<UserInfo>.FromSuccess(userInfo);
            }

            var errors = await JsonSerializer.DeserializeAsync<List<string>>(contentStream, DefaultDeserialisationOptions);
            return AuthenticationOperation<UserInfo>.FromError(errors);
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync(ApiBase + "Logout", null);
        }

        public async Task<UserInfo> GetUserAsync()
        {
            var result = await _httpClient.GetAsync(ApiBase + "User");
            var contentStream = await result.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<UserInfo>(contentStream, DefaultDeserialisationOptions);
        }

        public async Task<AuthenticationOperation> ForgotPasswordAsync(string email)
        {
            var request = new ForgotPasswordRequest { Email = email };
            var result = await _httpClient.PostAsJsonAsync(ApiBase + "ForgotPassword", request);
            

            if (result.IsSuccessStatusCode)
                return AuthenticationOperation.FromSuccess();

            var contentStream = await result.Content.ReadAsStreamAsync();
            var errors = await JsonSerializer.DeserializeAsync<List<string>>(contentStream, DefaultDeserialisationOptions);
            return AuthenticationOperation.FromError(errors);
        }

        public async Task<AuthenticationOperation> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var request = new ResetPasswordRequest { Email = email, Token = token, NewPassword = newPassword };
            var result = await _httpClient.PostAsJsonAsync(ApiBase + "ResetPassword", request);

            if (result.IsSuccessStatusCode)
                return AuthenticationOperation.FromSuccess();

            var contentStream = await result.Content.ReadAsStreamAsync();
            var errors = await JsonSerializer.DeserializeAsync<List<string>>(contentStream, DefaultDeserialisationOptions);
            return AuthenticationOperation.FromError(errors);
        }

        public async Task<AuthenticationOperation> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var request = new ChangePasswordRequest {CurrentPassword = currentPassword, NewPassword = newPassword};
            var request = new ChangePasswordRequest { CurrentPassword = currentPassword, NewPassword = newPassword };
            var result = await _httpClient.PostAsJsonAsync(ApiBase + "ChangePassword", request);

            if (result.IsSuccessStatusCode)
                return AuthenticationOperation.FromSuccess();

            var contentStream = await result.Content.ReadAsStreamAsync();
            var errors = await JsonSerializer.DeserializeAsync<List<string>>(contentStream, DefaultDeserialisationOptions);
            return AuthenticationOperation.FromError(errors);
        }
    }
}
