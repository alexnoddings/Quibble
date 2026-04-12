namespace Quibble.Games.Models;

public class FullSubmittedAnswer
{
    public Guid ParticipantId { get; set; }
    public Guid QuestionId { get; set; }
    public decimal? Points { get; set; }
    public string Answer { get; set; } = string.Empty;
}
