using System;

namespace Quibble.Shared.Entities
{
    public interface IQuestion : IEntity
    {
        public Guid RoundId { get; }
        public string Text { get; }
        public string Answer { get; }
        public sbyte Points { get; }
        public QuestionState State { get; }
    }
}
