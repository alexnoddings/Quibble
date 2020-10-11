using System;

namespace Quibble.Core.Entities
{
    public interface IQuestion : IEntity
    {
        public Guid RoundId { get; }

        public string QuestionText { get; }

        public string CorrectAnswer { get; }

        public QuestionState State { get; }
    }
}