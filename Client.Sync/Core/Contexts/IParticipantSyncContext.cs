using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Core.Contexts;

public interface IParticipantSyncContext
{
    public event Func<ParticipantDto, List<SubmittedAnswerDto>, Task> OnParticipantJoinedAsync;
}
