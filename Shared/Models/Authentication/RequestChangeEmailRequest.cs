using System.ComponentModel.DataAnnotations;

namespace Quibble.Shared.Models.Authentication;

public class RequestChangeEmailRequest
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string NewEmail { get; set; } = string.Empty;
}
