using System.ComponentModel.DataAnnotations;

namespace BlazorIdentityBase.Shared.Authentication
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
    }
}
