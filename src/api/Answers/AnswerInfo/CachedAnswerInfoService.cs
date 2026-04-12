using Quibble.Questions.Info;

namespace Quibble.Answers.Info;

internal sealed class CachedAnswerInfoService : IAnswerInfoService
{
    private readonly AnswerInfoCache _cache;

    public CachedAnswerInfoService(AnswerInfoCache cache)
    {
        _cache = cache;
    }

    public ValueTask<AnswerInfo?> GetAnswerToQuestionByParticipantAsync(Guid questionId, Guid participantId) =>
        _cache.GetAnswerToQuestionByParticipantAsync(questionId, participantId);

    public ValueTask<AnswerInfo?> GetAnswerToQuestionByUserAsync(Guid questionId, Guid userId) =>
        _cache.GetAnswerToQuestionByUserAsync(questionId, userId);

    public ValueTask OnQuestionStateUpdatedAsync(Guid participantId, Guid questionId) =>
        _cache.OnAnswerInfoStaleAsync(participantId, questionId);

    public ValueTask OnQuestionDeletedAsync(Guid participantId, Guid questionId) =>
        _cache.OnAnswerDeletedAsync(participantId, questionId);
}
