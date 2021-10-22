using Quibble.Shared.Entities;

namespace Quibble.Server.Data.Models;

public class DbQuiz : IQuiz
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public AppUser Owner { get; set; } = default!;

    public string Title { get; set; } = string.Empty;
    public QuizState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? OpenedAt { get; set; }

    public List<DbRound> Rounds { get; set; } = new();
    public List<DbParticipant> Participants { get; set; } = new();
}
