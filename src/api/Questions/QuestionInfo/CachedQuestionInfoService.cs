namespace Quibble.Questions.Info;

internal sealed class CachedQuestionInfoService : IQuestionInfoService
{
    private readonly QuestionInfoCache _cache;

    public CachedQuestionInfoService(QuestionInfoCache cache)
    {
        _cache = cache;
    }

    public ValueTask<QuestionInfo?> GetQuestionByIdAsync(Guid id) => _cache.GetQuestionByIdAsync(id);
    public ValueTask OnQuestionStateUpdatedAsync(Guid id) => _cache.OnQuestionInfoStaleAsync(id);
    public ValueTask OnQuestionDeletedAsync(Guid id) => _cache.OnQuestionDeletedAsync(id);
}
