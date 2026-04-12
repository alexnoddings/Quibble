using Quibble.Api.Games;
using Quibble.Api.Questions;
using Quibble.Api.Rounds;
using Quibble.Api.Users;

namespace Quibble.Api;

public static class EndpointFilterHelpers
{
    public static bool HasUserContextParameter(EndpointFilterFactoryContext factoryContext, out int index) =>
        HasParameterOf<UserContext>(factoryContext, out index);

    public static bool HasGameContextParameter(EndpointFilterFactoryContext factoryContext, out int index) =>
        HasParameterOf<GameContext>(factoryContext, out index);

    public static bool HasRoundContextParameter(EndpointFilterFactoryContext factoryContext, out int index) =>
        HasParameterOf<RoundContext>(factoryContext, out index);

    public static bool HasQuestionContextParameter(EndpointFilterFactoryContext factoryContext, out int index) =>
        HasParameterOf<QuestionContext>(factoryContext, out index);

    private static bool HasParameterOf<T>(EndpointFilterFactoryContext factoryContext, out int index)
    {
        var parameters = factoryContext.MethodInfo.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (parameter.ParameterType == typeof(T))
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }
}
