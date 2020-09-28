using System;
using Quibble.Core.Entities;

namespace Quibble.UI.Core.Entities
{
    public class DtoQuiz : IQuiz
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public bool IsPublished => PublishedAt != null;

        public DtoQuiz()
        {
        }

        public DtoQuiz(IQuiz other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            OwnerId = other.OwnerId;
            Title = other.Title;
            PublishedAt = other.PublishedAt;
        }
    }
}
