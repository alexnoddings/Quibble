using System;

namespace Quibble.Core.Entities
{
    public interface IRound : IEntity
    {
        public Guid QuizId { get; }

        public string Title { get; }

        public RoundState State { get; }
    }
}