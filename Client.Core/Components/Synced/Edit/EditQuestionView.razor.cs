﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Quibble.Client.Core;
using Quibble.Client.Core.Components.Modals;
using Quibble.Client.Core.Entities;

namespace Quibble.Client.Core.Components.Synced.Edit;

public sealed partial class EditQuestionView : IDisposable
{
	[Parameter]
	public ISyncedQuestion Question { get; set; } = default!;

	private OptionsModal<bool> ConfirmDeleteModal { get; set; } = default!;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		Question.Updated += OnUpdatedAsync;
	}

	private async Task OnDeleteClickedAsync(MouseEventArgs args)
	{
		if (args.ShiftKey
			|| string.IsNullOrWhiteSpace(Question.Text) && string.IsNullOrWhiteSpace(Question.Answer)
			|| await ConfirmDeleteModal.ShowAsync(false))
			await Question.DeleteAsync();
	}

	protected override int CalculateStateStamp() =>
		StateStamp.ForProperties(Question);

	public void Dispose()
	{
		Question.Updated -= OnUpdatedAsync;
	}
}
