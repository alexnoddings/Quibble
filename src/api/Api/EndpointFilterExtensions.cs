using Quibble.Api.Answers;
using Quibble.Api.Games;
using Quibble.Api.Questions;
using Quibble.Api.Rounds;
using Quibble.Api.Users;

namespace Quibble.Api;

public static class EndpointFilterExtensions
{
    public static TBuilder AddRequirementFilters<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilterFactory(RequireGameExistsEndpointFilter.Factory);
        builder.AddEndpointFilterFactory(RequireUserOwnsGameEndpointFilter.Factory);
        builder.AddEndpointFilterFactory(RequireGameStateEndpointFilter.Factory);

        builder.AddEndpointFilterFactory(RequireRoundExistsEndpointFilter.Factory);
        builder.AddEndpointFilterFactory(RequireRoundStateEndpointFilter.Factory);

        builder.AddEndpointFilterFactory(RequireQuestionExistsEndpointFilter.Factory);

        builder.AddEndpointFilterFactory(RequireAnswerExistsEndpointFilter.Factory);
        builder.AddEndpointFilterFactory(RequireUserIsParticipantEndpointFilter.Factory);

        return builder;
    }
}
