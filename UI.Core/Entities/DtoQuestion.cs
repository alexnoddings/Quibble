using System;
using Quibble.Core.Entities;

namespace Quibble.UI.Core.Entities
{
    public class DtoQuestion : IQuestion
    {
        public Guid Id { get; set; }
        public Guid RoundId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;

        public DtoQuestion()
        {
        }

        public DtoQuestion(IQuestion other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            RoundId = other.RoundId;
            QuestionText = other.QuestionText;
            CorrectAnswer = other.CorrectAnswer;
        }
    }
}
