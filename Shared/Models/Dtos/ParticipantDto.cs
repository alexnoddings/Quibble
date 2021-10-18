using Quibble.Shared.Entities;

namespace Quibble.Shared.Models.Dtos
{
    public class ParticipantDto : IParticipant
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsCurrentUser { get; set; }
    }
}
