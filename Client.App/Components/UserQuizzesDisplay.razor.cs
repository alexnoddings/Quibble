using Microsoft.AspNetCore.Components;
using Quibble.Client.App.Services.Quiz;
using Quibble.Common.Dtos;

namespace Quibble.Client.App.Components;

public partial class UserQuizzesDisplay
{
	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	[Inject]
	private IQuizService QuizService { get; set; } = default!;

	private UserQuizzesDto? UserQuizzes { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		UserQuizzes = await QuizService.GetUserQuizzesAsync();
	}

	private async Task CreateNewQuizAsync()
	{
		var newQuizId = await QuizService.CreateNewQuizAsync();
		NavigationManager.NavigateTo($"/quiz/{newQuizId}");
	}
}
