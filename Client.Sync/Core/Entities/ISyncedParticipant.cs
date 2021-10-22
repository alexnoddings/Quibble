using Quibble.Shared.Entities;

namespace Quibble.Client.Sync.Core.Entities;

public interface ISyncedParticipant : ISyncedEntity, IParticipant
{
    public ISyncedQuiz Quiz { get; }
    public ISyncedEntities<ISyncedSubmittedAnswer> Answers { get; }

    public string UserName { get; }
    public bool IsCurrentUser { get; }
}
