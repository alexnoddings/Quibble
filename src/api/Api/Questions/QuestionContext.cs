using Quibble.Questions.Info;

namespace Quibble.Api.Questions;

public sealed class QuestionContext
{
    public QuestionInfo Question { get; }

    public QuestionContext(QuestionInfo question)
    {
        Question = question;
    }

    public static ValueTask<QuestionContext?> BindAsync(HttpContext context)
    {
        if (context.QuibbleContext.Question is { } questionInfo)
        {
            var questionContext = new QuestionContext(questionInfo);
            return ValueTask.FromResult<QuestionContext?>(questionContext);
        }

        return ValueTask.FromResult<QuestionContext?>(null);
    }
}
