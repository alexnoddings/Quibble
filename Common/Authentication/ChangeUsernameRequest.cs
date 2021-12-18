using System.ComponentModel.DataAnnotations;

namespace Quibble.Common.Authentication;

public class ChangeUsernameRequest
{
	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; } = string.Empty;

	[Required]
	[DataType("Username")]
	public string NewUsername { get; set; } = string.Empty;
}
