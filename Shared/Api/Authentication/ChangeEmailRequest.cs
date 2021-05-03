using System.ComponentModel.DataAnnotations;

namespace Quibble.Shared.Api.Authentication
{
    public class ChangeEmailRequest
    {
        [Required]
        [DataType("Token")]
        public string Token { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; } = string.Empty;
    }
}
