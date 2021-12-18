using Microsoft.Extensions.Logging;
using Quibble.Server.Core.Sync;
using Quibble.Sync.InMemory.Client.Contexts;
using System.Diagnostics.CodeAnalysis;

namespace Quibble.Sync.InMemory.Server;

internal class SyncedUsers : ISyncedUsers
{
	private ILoggerFactory LoggerFactory { get; }
	private SyncContextStore ContextStore { get; set; }
	internal SyncContext? InvokingContext { get; set; }

	public SyncedUsers(ILoggerFactory loggerFactory, SyncContextStore contextStore)
	{
		LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
		ContextStore = contextStore ?? throw new ArgumentNullException(nameof(contextStore));
	}

	public Task AddCurrentUserAsHost()
	{
		EnsureInitialised();
		ContextStore.TryAdd(InvokingContext);
		return Task.CompletedTask;
	}

	public Task AddCurrentUserAsParticipant(Guid _)
	{
		EnsureInitialised();
		ContextStore.TryAdd(InvokingContext);
		return Task.CompletedTask;
	}

	public ISyncedEvents All()
	{
		EnsureInitialised();
		var quizId = InvokingContext.QuizId;
		return BuildSyncedEvents(ctx => ctx.QuizId == quizId);
	}

	public ISyncedEvents Hosts()
	{
		EnsureInitialised();
		var quizId = InvokingContext.QuizId;
		return BuildSyncedEvents(ctx => ctx.QuizId == quizId && ctx.ParticipantId == Guid.Empty);
	}

	public ISyncedEvents Participant(Guid participantId)
	{
		EnsureInitialised();
		var quizId = InvokingContext.QuizId;
		return BuildSyncedEvents(ctx => ctx.QuizId == quizId && ctx.ParticipantId == participantId);
	}

	public ISyncedEvents ParticipantExceptCaller(Guid participantId)
	{
		EnsureInitialised();
		var quizId = InvokingContext.QuizId;
		var invokingId = InvokingContext.InstanceId;
		return BuildSyncedEvents(ctx => ctx.QuizId == quizId && ctx.ParticipantId == participantId && ctx.InstanceId != invokingId);
	}

	public ISyncedEvents Participants()
	{
		EnsureInitialised();
		var quizId = InvokingContext.QuizId;
		return BuildSyncedEvents(ctx => ctx.QuizId == quizId && ctx.ParticipantId != Guid.Empty);
	}

	private SyncedEvents BuildSyncedEvents(Func<SyncContext, bool> predicate) =>
		new(LoggerFactory, ContextStore, predicate);

	[MemberNotNull(nameof(InvokingContext))]
	private void EnsureInitialised()
	{
		if (InvokingContext is null)
			throw new InvalidOperationException($"Not initialised; {nameof(InvokingContext)} must be set.");
	}
}
