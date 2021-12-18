using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Client.Core.Extensions;
using Quibble.Common.Dtos;
using Quibble.Common.Entities;
using Quibble.Server.Core.Domain;

namespace Quibble.Sync.InMemory.Client.Contexts;

internal class QuestionSyncContext : BaseSyncContext<QuestionLogic>, IQuestionSyncContext
{
	public event Func<QuestionDto, Task>? OnAddedAsync;
	public event Func<QuestionDto, List<SubmittedAnswerDto>, Task>? OnRevealedAsync;
	public event Func<Guid, string, Task>? OnTextUpdatedAsync;
	public event Func<Guid, string, Task>? OnAnswerUpdatedAsync;
	public event Func<Guid, decimal, Task>? OnPointsUpdatedAsync;
	public event Func<Guid, QuestionState, Task>? OnStateUpdatedAsync;
	public event Func<Guid, int, Task>? OnOrderUpdatedAsync;
	public event Func<Guid, Task>? OnDeletedAsync;

	public QuestionSyncContext(ILogger<QuestionSyncContext> logger, SyncContext parent)
		: base(logger, parent)
	{
	}

	public Task AddQuestionAsync(Guid roundId) =>
		InvokeAsync(questions => questions.CreateQuestionAsync(Context, roundId));
	internal Task InvokeOnAddedAsync(QuestionDto dto) =>
		OnAddedAsync.TryInvokeAsync(dto);

	public Task UpdateTextAsync(Guid id, string newText) =>
		InvokeAsync(questions => questions.UpdateQuestionTextAsync(Context, id, newText));
	internal Task InvokeOnTextUpdatedAsync(Guid id, string newText) =>
		OnTextUpdatedAsync.TryInvokeAsync(id, newText);

	public Task UpdateAnswerAsync(Guid id, string newAnswer) =>
		InvokeAsync(questions => questions.UpdateQuestionAnswerAsync(Context, id, newAnswer));
	internal Task InvokeOnAnswerUpdatedAsync(Guid id, string newAnswer) =>
		OnAnswerUpdatedAsync.TryInvokeAsync(id, newAnswer);

	public Task UpdatePointsAsync(Guid id, decimal newPoints) =>
		InvokeAsync(questions => questions.UpdateQuestionPointsAsync(Context, id, newPoints));
	internal Task InvokeOnPointsUpdatedAsync(Guid id, decimal newPoints) =>
		OnPointsUpdatedAsync.TryInvokeAsync(id, newPoints);

	public Task UpdateStateAsync(Guid id, QuestionState newState) =>
		InvokeAsync(questions => questions.UpdateQuestionStateAsync(Context, id, newState));
	internal Task InvokeOnStateUpdatedAsync(Guid id, QuestionState newState) =>
		OnStateUpdatedAsync.TryInvokeAsync(id, newState);
	internal Task InvokeOnRevealedAsync(QuestionDto dto, List<SubmittedAnswerDto> submittedAnswerDtos) =>
		OnRevealedAsync.TryInvokeAsync(dto, submittedAnswerDtos);

	internal Task InvokeOnOrderUpdatedAsync(Guid id, int newOrder) =>
		OnOrderUpdatedAsync.TryInvokeAsync(id, newOrder);

	public Task DeleteAsync(Guid id) => InvokeAsync(questions =>
		questions.DeleteQuestionAsync(Context, id));
	internal Task InvokeOnDeletedAsync(Guid id) =>
		OnDeletedAsync.TryInvokeAsync(id);
}
