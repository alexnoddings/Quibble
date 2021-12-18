using Microsoft.AspNetCore.Components;
using Quibble.Client.Core;
using Quibble.Client.Core.Entities;

namespace Quibble.Client.Core.Components.Synced.Take;

public sealed partial class TakeQuizView : IDisposable
{
	[Parameter]
	public ISyncedQuiz Quiz { get; set; } = default!;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		Quiz.Updated += OnUpdatedAsync;
	}

	protected override int CalculateStateStamp() =>
		StateStamp.ForProperties(Quiz);

	public void Dispose()
	{
		Quiz.Updated -= OnUpdatedAsync;
	}
}
