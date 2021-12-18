using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Entities;
using Quibble.Client.Core.Factories;
using Quibble.Common.Api;

namespace Quibble.Client.App.Pages;

[Authorize]
public sealed partial class Quiz : IAsyncDisposable
{
	[Parameter]
	public Guid QuizId { get; set; }

	[Inject]
	private ISyncedQuizFactory QuizFactory { get; set; } = default!;

	[CascadingParameter]
	private Task<Guid> UserIdTask { get; set; } = default!;

	private Guid UserId { get; set; }

	private ApiResponse<ISyncedQuiz>? GetQuizResult { get; set; }

	protected override async Task OnInitializedAsync()
	{
		UserId = await UserIdTask;
		GetQuizResult = await QuizFactory.GetSyncedQuizAsync(QuizId);

		if (GetQuizResult.Value is not null)
			GetQuizResult.Value.Updated += OnQuizUpdated;
	}

	private Task OnQuizUpdated() => InvokeAsync(StateHasChanged);

	public async ValueTask DisposeAsync()
	{
		var syncedQuiz = GetQuizResult?.Value;
		if (syncedQuiz is null) return;

		syncedQuiz.Updated -= OnQuizUpdated;
		await syncedQuiz.DisposeAsync();
		GetQuizResult = null;
	}
}
