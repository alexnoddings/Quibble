using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Client.Core.Extensions;
using Quibble.Common.Api;
using Quibble.Common.Dtos;
using Quibble.Server.Core;
using Quibble.Server.Core.Domain;

namespace Quibble.Sync.InMemory.Client.Contexts;

internal class QuizSyncContext : BaseSyncContext<QuizLogic>, IQuizSyncContext
{
	public event Func<string, Task>? OnTitleUpdatedAsync;
	public event Func<Task>? OnOpenedAsync;
	public event Func<Task>? OnDeletedAsync;

	public QuizSyncContext(ILogger<QuizSyncContext> logger, SyncContext parent)
		: base(logger, parent)
	{
	}

	public async Task<ApiResponse<(FullQuizDto Quiz, Guid ParticipantId)>> InitialiseAsync()
	{
		await InvokeAsync(quizzes => quizzes.OnUserConnectedAsync(Context));

		var userId = Context.UserId;

		using var scope = Parent.ScopeFactory.CreateScope();

		var quizLogic = scope.ServiceProvider.GetRequiredService<QuizLogic>();
		await quizLogic.OnUserConnectedAsync(Context);
		var quizDtoResponse = await quizLogic.GetQuizAsync(Context);

		if (!quizDtoResponse.WasSuccessful)
			return ApiResponse.FromError<(FullQuizDto, Guid)>(quizDtoResponse.Error);

		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var participant =
			await dbContext.Participants.FirstOrDefaultAsync(participant =>
				participant.QuizId == Context.QuizId
				&& participant.UserId == Context.UserId);

		return ApiResponse.FromSuccess((quizDtoResponse.Value, participant?.Id ?? Guid.Empty));
	}

	public Task GetQuizAsync() =>
		InvokeAsync(quizzes => quizzes.GetQuizAsync(Context));

	public Task OpenAsync() =>
		InvokeAsync(quizzes => quizzes.OpenQuizAsync(Context));
	internal Task InvokeOnOpenedAsync() =>
		OnOpenedAsync.TryInvokeAsync();

	public Task UpdateTitleAsync(string newTitle) =>
		InvokeAsync(quizzes => quizzes.UpdateQuizTitleAsync(Context, newTitle));
	internal Task InvokeOnTitleUpdatedAsync(string newTitle) =>
		OnTitleUpdatedAsync.TryInvokeAsync(newTitle);

	public Task DeleteAsync() =>
		InvokeAsync(quizzes => quizzes.DeleteQuizAsync(Context));
	internal Task InvokeOnDeletedAsync() =>
		OnDeletedAsync.TryInvokeAsync();
}
