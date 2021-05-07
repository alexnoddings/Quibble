using System;

namespace Quibble.Shared.Entities
{
    public interface IParticipant : IEntity
    {
        public Guid QuizId { get; }
    }
}
