namespace Quibble.Games.Models;

public class PartialQuestionAnswer
{
    public string? Answer { get; set; }

    public PartialSubmittedAnswer SubmittedAnswer { get; set; } = null!;
}
