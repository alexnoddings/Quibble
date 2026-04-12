using Quibble.Answers.Info;
using Quibble.Caching;
using Quibble.Games.Caching;
using Quibble.Games.Info;
using Quibble.Questions.Info;
using Quibble.Rounds.Info;
using Quibble.Users;

namespace Quibble.Hosting;

public static class Caching
{
    public static TBuilder AddQuibbleCaching<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.AddRedisDistributedCache(connectionName: "redis");
        builder.Services.AddHybridCache();
        builder.Services.AddScoped<QuibbleHybridCache>();

        builder.Services.AddScoped<CacheMetrics>();

        builder.Services.AddScoped<IUserInfoCache, UserInfoCache>();

        builder.Services.AddScoped<GameInfoCacheById>();
        builder.Services.AddScoped<GameInfoCacheBySlug>();
        builder.Services.AddScoped<IGameInfoService, CombinedCachedGameInfoService>();

        builder.Services.AddScoped<RoundInfoCache>();
        builder.Services.AddScoped<IRoundInfoService, CachedRoundInfoService>();

        builder.Services.AddScoped<QuestionInfoCache>();
        builder.Services.AddScoped<IQuestionInfoService, CachedQuestionInfoService>();

        builder.Services.AddScoped<AnswerInfoCache>();
        builder.Services.AddScoped<IAnswerInfoService, CachedAnswerInfoService>();

        return builder;
    }
}
