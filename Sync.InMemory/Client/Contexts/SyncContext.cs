using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Sync.InMemory.Server;

namespace Quibble.Sync.InMemory.Client.Contexts;

internal class SyncContext : ISyncContext
{
	internal Guid QuizId { get; }
	internal Guid ParticipantId { get; set; }
	internal Guid InstanceId { get; } = Guid.NewGuid();

	internal SyncContextStore SyncContextStore { get; }
	internal SyncExecutionContext SyncExecutionContext { get; }
	internal IServiceScopeFactory ScopeFactory { get; }

	internal QuizSyncContext QuizContext { get; }
	public IQuizSyncContext Quizzes => QuizContext;

	internal RoundSyncContext RoundContext { get; }
	public IRoundSyncContext Rounds => RoundContext;

	internal QuestionSyncContext QuestionContext { get; }
	public IQuestionSyncContext Questions => QuestionContext;

	internal SubmittedAnswerSyncContext SubmittedAnswerContext { get; }
	public ISubmittedAnswerSyncContext SubmittedAnswers => SubmittedAnswerContext;

	internal ParticipantSyncContext ParticipantContext { get; }
	public IParticipantSyncContext Participants => ParticipantContext;

	public SyncContext(ILoggerFactory loggerFactory, Guid quizId, SyncContextStore syncContextStore, SyncExecutionContext syncExecutionContext, IServiceScopeFactory scopeFactory)
	{
		QuizId = quizId;
		SyncContextStore = syncContextStore;
		SyncExecutionContext = syncExecutionContext;
		ScopeFactory = scopeFactory;

		QuizContext = new QuizSyncContext(loggerFactory.CreateLogger<QuizSyncContext>(), this);
		RoundContext = new RoundSyncContext(loggerFactory.CreateLogger<RoundSyncContext>(), this);
		QuestionContext = new QuestionSyncContext(loggerFactory.CreateLogger<QuestionSyncContext>(), this);
		SubmittedAnswerContext = new SubmittedAnswerSyncContext(loggerFactory.CreateLogger<SubmittedAnswerSyncContext>(), this);
		ParticipantContext = new ParticipantSyncContext(loggerFactory.CreateLogger<ParticipantSyncContext>(), this);
	}

	public ValueTask DisposeAsync()
	{
		SyncContextStore.Remove(this);
		return ValueTask.CompletedTask;
	}
}
