namespace Quibble.Games.Models;

public class PartialParticipant
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Points { get; set; }
}
