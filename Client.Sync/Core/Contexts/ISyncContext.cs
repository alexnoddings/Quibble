namespace Quibble.Client.Sync.Core.Contexts;

public interface ISyncContext : IAsyncDisposable
{
    public IQuizSyncContext Quizzes { get; }
    public IRoundSyncContext Rounds { get; }
    public IQuestionSyncContext Questions { get; }
    public ISubmittedAnswerSyncContext SubmittedAnswers { get; }
    public IParticipantSyncContext Participants { get; }
}
