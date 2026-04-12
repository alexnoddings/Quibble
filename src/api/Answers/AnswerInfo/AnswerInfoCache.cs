using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Quibble.Caching;
using Quibble.Data;
using Quibble.Data.Entites.Answers;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;
using Quibble.Data.Entites.Rounds;
using Quibble.Games.Info;
using Quibble.Questions.Info;

namespace Quibble.Answers.Info;

internal sealed class AnswerInfoCache
{
    private static readonly HybridCacheEntryOptions _cacheOptions = new()
    {
        LocalCacheExpiration = TimeSpan.FromSeconds(45),
        Expiration = TimeSpan.FromSeconds(90)
    };

    private readonly QuibbleHybridCache _cache;
    private readonly IDbContextFactory<QuibbleDbContext> _dbContextFactory;

    public AnswerInfoCache(QuibbleHybridCache cache, IDbContextFactory<QuibbleDbContext> dbContextFactory)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
    }

    // otherId uses participant or user id
    // we rely on guid sparsity meaning there will be practically no overlap here
    private static string GetAnswerCacheKey(Guid questionId, Guid otherId) =>
        CacheKey.For<AnswerInfo>.WithCompositeId(questionId, otherId);

    public ValueTask OnAnswerInfoStaleAsync(Guid participantId, Guid questionId)
    {
        var cacheKey = GetAnswerCacheKey(participantId, questionId);
        return _cache.RemoveAsync(cacheKey);
    }

    public ValueTask OnAnswerDeletedAsync(Guid participantId, Guid questionId)
    {
        var cacheTag = GetAnswerCacheKey(participantId, questionId);
        return _cache.RemoveByTagAsync(cacheTag);
    }

    public ValueTask<AnswerInfo?> GetAnswerToQuestionByParticipantAsync(Guid questionId, Guid participantId)
    {
        var cacheKey = GetAnswerCacheKey(questionId, participantId);
        return GetAnswerAsync(
            cacheKey,
            a => a.QuestionAnswer.QuestionId == questionId && a.ParticipantId == participantId
        );
    }

    public ValueTask<AnswerInfo?> GetAnswerToQuestionByUserAsync(Guid questionId, Guid userId)
    {
        var cacheKey = GetAnswerCacheKey(questionId, userId);
        return GetAnswerAsync(
            cacheKey,
            a => a.QuestionAnswer.QuestionId == questionId && a.Participant.UserId == userId
        );
    }

    private async ValueTask<AnswerInfo?> GetAnswerAsync(
        string cacheKey,
        Expression<Func<SubmittedAnswer, bool>> filter
    )
    {
        var answerInfo = await _cache.GetOrCreateAsync(
            cacheKey,
            filter,
            LoadAnswerRawAsync,
            static answerInfo =>
            [
                CacheKey.For<Game>.WithId(answerInfo.Question.Round.Game.Id),
                CacheKey.For<Round>.WithId(answerInfo.Question.Round.Id),
                CacheKey.For<Question>.WithId(answerInfo.Question.Id),
                GetAnswerCacheKey(answerInfo.Participant.Id, answerInfo.Question.Id),
            ],
            _cacheOptions
        );

        return answerInfo;
    }

    private async ValueTask<Cached<AnswerInfo>?> LoadAnswerRawAsync(
        Expression<Func<SubmittedAnswer, bool>> filter,
        Guid nonce,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var answer = await dbContext
            .Answers
            .AsNoTracking()
            .Where(filter)
            .Select(a => new AnswerInfo
            {
                Participant = new()
                {
                    Id = a.Participant.Id,
                    UserId = a.Participant.UserId,
                },
                Question = new()
                {
                    Id = a.QuestionAnswer.Question.Id,
                    State = a.QuestionAnswer.Question.State,
                    Round = new()
                    {
                        Id = a.QuestionAnswer.Question.Round.Id,
                        State = a.QuestionAnswer.Question.Round.State,
                        Game = new()
                        {
                            Id = a.QuestionAnswer.Question.Round.GameId,
                            Slug = a.QuestionAnswer.Question.Round.Game.Slug,
                            OwnerId = a.QuestionAnswer.Question.Round.Game.OwnerId,
                            State = a.QuestionAnswer.Question.Round.Game.State,
                            Participants = a.QuestionAnswer.Question.Round.Game.Participants
                                .Select(p => new GameParticipantInfo { Id = p.Id, UserId = p.UserId })
                                .ToList()
                                .AsReadOnly()
                        }
                    }
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (answer is null)
            return null;

        return new Cached<AnswerInfo> { CacheNonce = nonce, Value = answer };
    }
}
