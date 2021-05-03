using Quibble.Shared.Entities;

namespace Quibble.Shared.Models
{
    public class QuizNegotiationDto
    {
        public bool CanEdit { get; set; }
        public QuizState State { get; set; }
    }
}
