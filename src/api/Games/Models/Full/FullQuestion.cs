using Quibble.Data.Entites.Questions;

namespace Quibble.Games.Models;

public class FullQuestion
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public QuestionState State { get; set; }
    public decimal Points { get; set; }

    public FullQuestionBody Body { get; set; } = null!;
    public FullQuestionAnswer Answer { get; set; } = null!;
}
