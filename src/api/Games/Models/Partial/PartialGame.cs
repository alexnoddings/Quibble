using Quibble.Data.Entites.Games;

namespace Quibble.Games.Models;

public class PartialGame
{
    public GamePerspective Perspective => GamePerspective.Participant;

    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public GameState State { get; set; }
    public string Title { get; set; } = string.Empty;

    public List<PartialParticipant> Participants { get; set; } = [];
    public List<PartialRound> Rounds { get; set; } = [];
}
