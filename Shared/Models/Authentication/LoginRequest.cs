using System.ComponentModel.DataAnnotations;

namespace Quibble.Shared.Models.Authentication
{
    public class LoginRequest
    {
        [Required]
        [DataType("Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
