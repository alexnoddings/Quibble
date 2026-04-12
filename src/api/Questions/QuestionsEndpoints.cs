using Quibble.Questions.Endpoints;

namespace Quibble.Questions;

public static class QuestionsEndpoints
{
    public static IEndpointRouteBuilder MapQuestionsApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapCreateQuestion()
            .MapUpdateQuestionBodyText()
            .MapUpdateQuestionAnswerText()
            .MapUpdateQuestionPoints()
            .MapUpdateQuestionState()
            .MapDeleteQuestion();

        return endpoints;
    }
}
