using Quibble.Common.Dtos;

namespace Quibble.Client.Core.Contexts;

public interface IParticipantSyncContext
{
	public event Func<ParticipantDto, List<SubmittedAnswerDto>, Task> OnParticipantJoinedAsync;
}
