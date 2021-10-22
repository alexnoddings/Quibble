using Quibble.Shared.Entities;

namespace Quibble.Server.Data.Models;

public class DbQuestion : IQuestion
{
    public Guid Id { get; set; }
    public Guid RoundId { get; set; }
    public DbRound Round { get; set; } = default!;

    public string Text { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public decimal Points { get; set; }
    public QuestionState State { get; set; }
    public int Order { get; set; }

    public List<DbSubmittedAnswer> SubmittedAnswers { get; set; } = new();
}
