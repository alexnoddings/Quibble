using System;

namespace Quibble.Core.Entities
{
    public interface IParticipant : IEntity
    {
        public Guid UserId { get; }

        public Guid QuizId { get; }

        public string UserName { get; }
    }
}