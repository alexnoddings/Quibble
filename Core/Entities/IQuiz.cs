using System;

namespace Quibble.Core.Entities
{
    public interface IQuiz : IEntity
    {

        public Guid OwnerId { get; }

        public string Title { get; }

        public DateTime? PublishedAt { get; }

        public bool IsPublished { get; }
    }
}