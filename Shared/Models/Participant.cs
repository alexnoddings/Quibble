using System;

namespace Quibble.Shared.Models
{
    public class Participant : IEntity
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
    }
}
