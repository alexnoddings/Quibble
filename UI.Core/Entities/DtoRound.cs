using System;
using Quibble.Core.Entities;

namespace Quibble.UI.Core.Entities
{
    public class DtoRound : IRound
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Title { get; set; } = string.Empty;

        public DtoRound()
        {
        }

        public DtoRound(IRound other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            QuizId = other.QuizId;
            Title = other.Title;
        }
    }
}
