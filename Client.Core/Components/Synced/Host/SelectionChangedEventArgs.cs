using Quibble.Client.Core.Entities;

namespace Quibble.Client.Core.Components.Synced.Host;

public class SelectionChangedEventArgs : EventArgs
{
	public ISyncedQuestion PreviousQuestion { get; }
	public ISyncedQuestion NewQuestion { get; }

	public SelectionChangedEventArgs(ISyncedQuestion previousQuestion, ISyncedQuestion newQuestion)
	{
		PreviousQuestion = previousQuestion ?? throw new ArgumentNullException(nameof(previousQuestion));
		NewQuestion = newQuestion ?? throw new ArgumentNullException(nameof(newQuestion));
	}
}
