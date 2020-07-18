using System;

namespace Quibble.Common.Participants
{
    public class Participant : IEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Guid QuizId { get; set; }

        public string NickName { get; set; } = string.Empty;
    }
}
