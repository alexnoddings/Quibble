using Quibble.Data.Entites.Games;

namespace Quibble.Games.Models;

public class FullGame
{
    public GamePerspective Perspective => GamePerspective.Host;

    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public GameState State { get; set; }
    public string Title { get; set; } = string.Empty;

    public List<FullParticipant> Participants { get; set; } = [];
    public List<FullRound> Rounds { get; set; } = [];
}
