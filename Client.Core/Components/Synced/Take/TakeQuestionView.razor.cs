using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Entities;
using Quibble.Client.Core.Extensions;

namespace Quibble.Client.Core.Components.Synced.Take;

public sealed partial class TakeQuestionView : IDisposable
{
	[Parameter]
	public ISyncedQuestion Question { get; set; } = default!;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		Question.Updated += OnUpdatedAsync;
		var answer = Question.TryGetCurrentUsersAnswer();
		if (answer is not null)
			answer.Updated += OnUpdatedAsync;
	}

	protected override int CalculateStateStamp() =>
		StateStamp.ForProperties(Question);

	public void Dispose()
	{
		Question.Updated -= OnUpdatedAsync;
		var answer = Question.TryGetCurrentUsersAnswer();
		if (answer is not null)
			answer.Updated -= OnUpdatedAsync;
	}
}
