using System.ComponentModel.DataAnnotations;

namespace Quibble.Common.Authentication;

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
