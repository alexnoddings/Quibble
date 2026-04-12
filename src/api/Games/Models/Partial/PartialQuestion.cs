using Quibble.Data.Entites.Questions;

namespace Quibble.Games.Models;

public class PartialQuestion
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public QuestionState State { get; set; }
    public decimal Points { get; set; }

    public PartialQuestionBody Body { get; set; } = null!;
    public PartialQuestionAnswer Answer { get; set; } = null!;
}
