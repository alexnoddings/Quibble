using Quibble.Data.Entites.Rounds;

namespace Quibble.Games.Models;

public class FullRound
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public RoundState State { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<FullQuestion> Questions { get; set; } = [];
}
