using Quibble.Client.Sync.Core.Entities;

namespace Quibble.Client.Pages.Quiz.Host;

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
