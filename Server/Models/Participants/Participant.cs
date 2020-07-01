using System;

namespace Quibble.Server.Models.Participants
{
    public class Participant : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Guid QuizId { get; set; }

        public string Name { get; set; } = "User";
    }
}
 