using System;

namespace Quibble.Core.Entities
{
    public interface IParticipantAnswer : IEntity
    {
        public Guid QuestionId { get; }

        public Guid ParticipantId { get; }

        public string Text { get; }

        public AnswerMark Mark { get; }
    }
}