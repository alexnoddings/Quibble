using System.ComponentModel.DataAnnotations;

namespace Quibble.Shared.Models.Authentication
{
    public class RegisterRequest
    {
        [Required]
        [DataType("Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
