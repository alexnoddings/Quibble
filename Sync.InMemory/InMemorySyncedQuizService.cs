using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Common.Api;
using Quibble.Common.Dtos;
using Quibble.Sync.InMemory.Client.Contexts;
using Quibble.Sync.InMemory.Server;

namespace Quibble.Sync.InMemory;

internal class InMemorySyncedQuizService : IInMemorySyncedQuizService
{
	private ILogger<InMemorySyncedQuizService> Logger { get; }
	private ILoggerFactory LoggerFactory { get; }
	private IServiceScopeFactory ScopeFactory { get; }
	private SyncContextStore ContextStore { get; }

	private Guid? UserId { get; set; }

	public InMemorySyncedQuizService(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory, SyncContextStore contextStore)
	{
		Logger = loggerFactory.CreateLogger<InMemorySyncedQuizService>();
		LoggerFactory = loggerFactory;
		ScopeFactory = scopeFactory;
		ContextStore = contextStore;
	}

	public Task<ApiResponse<(FullQuizDto, ISyncContext)>> GetQuizAsync(Guid id)
	{
		if (UserId is null)
			throw new InvalidOperationException($"Must first set user id with {nameof(UseUser)}.");

		return GetQuizAsUserAsync(id, UserId.Value);
	}

	public IDisposable UseUser(Guid userId)
	{
		if (UserId is not null)
			throw new InvalidOperationException($"User ID currently set elsewhere.");

		UserId = userId;
		return new DiscardUserDisposable(() => UserId = null);
	}

	private async Task<ApiResponse<(FullQuizDto, ISyncContext)>> GetQuizAsUserAsync(Guid quizId, Guid userId)
	{
		Logger.LogInformation("Getting quiz {QuizId} as {UserId}.", quizId, userId);

		if (quizId == Guid.Empty)
		{
			Logger.LogWarning(nameof(quizId) + " was empty.");
			return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);
		}

		if (userId == Guid.Empty)
		{
			Logger.LogWarning(nameof(userId) + " was empty.");
			return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(ApiErrors.QuizNotFound);
		}

		var users = new SyncedUsers(LoggerFactory, ContextStore);
		var executionContext = new SyncExecutionContext(quizId, userId, users);
		var syncContext = new SyncContext(LoggerFactory, quizId, ContextStore, executionContext, ScopeFactory);
		users.InvokingContext = syncContext;

		var initResponse = await syncContext.QuizContext.InitialiseAsync();
		if (!initResponse.WasSuccessful)
			return ApiResponse.FromError<(FullQuizDto, ISyncContext)>(initResponse.Error);

		var (dto, participantId) = initResponse.Value;
		syncContext.ParticipantId = participantId;

		return ApiResponse.FromSuccess<(FullQuizDto, ISyncContext)>((dto, syncContext));
	}

	private class DiscardUserDisposable : IDisposable
	{
		private Action DiscardAction { get; }

		public DiscardUserDisposable(Action discardAction)
		{
			DiscardAction = discardAction;
		}

		public void Dispose() => DiscardAction();
	}
}
