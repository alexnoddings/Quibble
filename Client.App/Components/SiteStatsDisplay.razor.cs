using Microsoft.AspNetCore.Components;
using Quibble.Client.App.Services.Quiz;
using Quibble.Common.Dtos;

namespace Quibble.Client.App.Components;

public partial class SiteStatsDisplay
{
	[Inject]
	private IQuizService QuizService { get; set; } = default!;

	private SiteStatsDto? SiteStats { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		SiteStats = await QuizService.GetSiteStatsAsync();
	}
}
