using Quibble.Answers.Endpoints;

namespace Quibble.Answers;

public static class AnswersEndpoints
{
    public static IEndpointRouteBuilder MapAnswersApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapUpdateAnswerText()
            .MapPreviewAnswerText()
            .MapUpdateAnswerPoints();

        return endpoints;
    }
}
