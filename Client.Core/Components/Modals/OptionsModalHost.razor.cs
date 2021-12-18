using Blazorise;
using Microsoft.AspNetCore.Components;

namespace Quibble.Client.Core.Components.Modals;

public partial class OptionsModalHost
{
	[Parameter]
	public RenderFragment ChildContent { get; set; } = default!;

	private string? Title { get; set; }
	private RenderFragment? Body { get; set; }
	private RenderFragment? Footer { get; set; }
	private bool CanBeClosed { get; set; }
	private object? DismissValue { get; set; }

	private Modal Modal { get; set; } = default!;

	// Allows ConfirmDeletionAsync to await a result from the modal
	private TaskCompletionSource<object?>? TaskCompletionSource { get; set; }

	private void OnModalClosing(ModalClosingEventArgs args)
	{
		if (CanBeClosed)
			TaskCompletionSource?.TrySetResult(DismissValue);
		else
			args.Cancel = true;
	}

	private void Choose(object? obj)
	{
		CanBeClosed = true;
		TaskCompletionSource?.TrySetResult(obj);
		Modal.Hide();
	}

	public async Task<TValue?> ShowAsync<TValue>(string title, RenderFragment body, RenderFragment<OptionsModalContext<TValue?>> footer, bool canBeDismissed, TValue? dismissValue = default)
	{
		if (Modal.Visible)
			throw new InvalidOperationException();

		Title = title;
		Body = body;
		Footer = builder =>
		{
			var context = new OptionsModalContext<TValue?>(choice => Choose(choice));
			footer.Invoke(context).Invoke(builder);
		};
		CanBeClosed = canBeDismissed;
		DismissValue = dismissValue;

		TaskCompletionSource = new TaskCompletionSource<object?>();
		Modal.Show();
		var returnValue = await TaskCompletionSource.Task;
		TaskCompletionSource = null;

		if (returnValue is TValue value)
			return value;
		return dismissValue;
	}
}
