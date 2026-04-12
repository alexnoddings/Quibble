using Quibble.Questions.Info;

namespace Quibble.Api.Answers;

public sealed class AnswerContext
{
    public AnswerInfo Answer { get; }

    public AnswerContext(AnswerInfo answer)
    {
        Answer = answer;
    }

    public static ValueTask<AnswerContext?> BindAsync(HttpContext context)
    {
        var answerContext = context.QuibbleContext.Answer switch
        {
            { } answerInfo => new AnswerContext(answerInfo),
            _ => null
        };
        return ValueTask.FromResult(answerContext);
    }
}
