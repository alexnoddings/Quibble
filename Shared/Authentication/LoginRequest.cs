using System.ComponentModel.DataAnnotations;

namespace BlazorIdentityBase.Shared.Authentication
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool ShouldRememberUser { get; set; }
    }
}
