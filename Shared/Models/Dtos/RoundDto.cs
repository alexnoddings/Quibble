using Quibble.Shared.Entities;

namespace Quibble.Shared.Models.Dtos
{
    public class RoundDto : IRound
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public RoundState State { get; set; }
        public int Order { get; set; }
    }
}
