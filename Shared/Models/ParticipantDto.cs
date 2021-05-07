using System;
using Quibble.Shared.Entities;

namespace Quibble.Shared.Models
{
    public class ParticipantDto : IParticipant
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
    }
}
