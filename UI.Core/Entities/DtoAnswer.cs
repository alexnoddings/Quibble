using Quibble.Core.Entities;
using System;

namespace Quibble.UI.Core.Entities
{
    public class DtoAnswer : IParticipantAnswer
    {
        public Guid Id { get; }

        public Guid QuestionId { get; }

        public Guid ParticipantId { get; }

        public string Text { get; } = string.Empty;

        public AnswerMark Mark { get; }

        public DtoAnswer()
        {
        }

        public DtoAnswer(IParticipantAnswer other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            QuestionId = other.QuestionId;
            ParticipantId = other.ParticipantId;
            Text = other.Text;
            Mark = other.Mark;
        }
    }
}
