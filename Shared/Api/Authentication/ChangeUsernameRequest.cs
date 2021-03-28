using System.ComponentModel.DataAnnotations;

namespace Quibble.Shared.Authentication
{
    public class ChangeUsernameRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType("Username")]
        public string NewUsername { get; set; } = string.Empty;
    }
}
