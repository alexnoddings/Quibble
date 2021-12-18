using Quibble.Common.Entities;

namespace Quibble.Common.Dtos;

public class QuizNegotiationDto
{
	public bool CanEdit { get; set; }
	public QuizState State { get; set; }
}
