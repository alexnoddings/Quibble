using Quibble.Common.Entities;

namespace Quibble.Common.Dtos;

public class ParticipantDto : IParticipant
{
	public Guid Id { get; set; }
	public Guid QuizId { get; set; }
	public string UserName { get; set; } = string.Empty;
	public bool IsCurrentUser { get; set; }
}
