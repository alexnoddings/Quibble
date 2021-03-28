using System.ComponentModel.DataAnnotations;

namespace Quibble.Shared.Authentication
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
    }
}
