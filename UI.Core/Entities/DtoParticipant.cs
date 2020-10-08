using System;
using Quibble.Core.Entities;

namespace Quibble.UI.Core.Entities
{
    public class DtoParticipant : IParticipant
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public Guid QuizId { get; }
        public string UserName { get; } = string.Empty;

        public DtoParticipant()
        {
        }

        public DtoParticipant(IParticipant other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            UserId = other.UserId;
            QuizId = other.QuizId;
            UserName = other.UserName;
        }
    }
}
