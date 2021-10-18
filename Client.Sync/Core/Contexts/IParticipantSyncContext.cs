using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Contexts
{
    public interface IParticipantSyncContext
    {
        public event Func<ParticipantDto, List<SubmittedAnswerDto>, Task> OnParticipantJoinedAsync;
    }
}
