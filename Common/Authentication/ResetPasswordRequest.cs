using System.ComponentModel.DataAnnotations;

namespace Quibble.Common.Authentication;

public class ResetPasswordRequest
{
	[Required]
	[EmailAddress]
	[DataType(DataType.EmailAddress)]
	public string Email { get; set; } = string.Empty;

	[Required]
	[DataType("Token")]
	public string Token { get; set; } = string.Empty;

	[Required]
	[DataType(DataType.Password)]
	public string NewPassword { get; set; } = string.Empty;
}
