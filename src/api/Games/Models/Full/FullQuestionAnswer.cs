namespace Quibble.Games.Models;

public class FullQuestionAnswer
{
    public string Answer { get; set; } = string.Empty;

    public List<FullSubmittedAnswer> SubmittedAnswers { get; set; } = [];
}
