using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Quibble.Caching;
using Quibble.Data;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;
using Quibble.Data.Entites.Rounds;
using Quibble.Games.Info;

namespace Quibble.Questions.Info;

internal sealed class QuestionInfoCache
{
    private static readonly HybridCacheEntryOptions _cacheOptions = new()
    {
        LocalCacheExpiration = TimeSpan.FromSeconds(45),
        Expiration = TimeSpan.FromSeconds(90)
    };

    private readonly QuibbleHybridCache _cache;
    private readonly IDbContextFactory<QuibbleDbContext> _dbContextFactory;

    public QuestionInfoCache(QuibbleHybridCache cache, IDbContextFactory<QuibbleDbContext> dbContextFactory)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
    }

    public ValueTask OnQuestionInfoStaleAsync(Guid id)
    {
        var cacheKey = CacheKey.For<QuestionInfo>.WithId(id);
        return _cache.RemoveAsync(cacheKey);
    }

    public ValueTask OnQuestionDeletedAsync(Guid id)
    {
        var cacheTag = CacheKey.For<Question>.WithId(id);
        return _cache.RemoveByTagAsync(cacheTag);
    }

    public async ValueTask<QuestionInfo?> GetQuestionByIdAsync(Guid id)
    {
        var cacheKey = CacheKey.For<QuestionInfo>.WithId(id);
        var questionInfo = await _cache.GetOrCreateAsync(
            cacheKey,
            id,
            LoadQuestionRawAsync,
            static questionInfo =>
            [
                CacheKey.For<Game>.WithId(questionInfo.Round.Game.Id),
                CacheKey.For<Round>.WithId(questionInfo.Round.Id),
                CacheKey.For<Question>.WithId(questionInfo.Id),
            ],
            _cacheOptions
        );

        return questionInfo;
    }

    private async ValueTask<Cached<QuestionInfo>?> LoadQuestionRawAsync(Guid id, Guid nonce, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var question = await dbContext
            .Questions
            .AsNoTracking()
            .Where(q => q.Id == id)
            .Select(q => new QuestionInfo
            {
                Id = q.Id,
                State = q.State,
                Round = new()
                {
                    Id = q.Round.Id,
                    State = q.Round.State,
                    Game = new()
                    {
                        Id = q.Round.GameId,
                        Slug = q.Round.Game.Slug,
                        OwnerId = q.Round.Game.OwnerId,
                        State = q.Round.Game.State,
                        Participants = q.Round.Game.Participants
                            .Select(p => new GameParticipantInfo { Id = p.Id, UserId = p.UserId, })
                            .ToList()
                            .AsReadOnly()
                    },
                }
                // BodyType = q.Body.Type,
                // AnswerType = q.Answer.Type
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (question is null)
            return null;

        return new Cached<QuestionInfo> { CacheNonce = nonce, Value = question };
    }
}
