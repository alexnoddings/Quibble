using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Client.Core.Extensions;
using Quibble.Common.Dtos;

namespace Quibble.Sync.InMemory.Client.Contexts;

internal class ParticipantSyncContext : BaseSyncContext, IParticipantSyncContext
{
	public event Func<ParticipantDto, List<SubmittedAnswerDto>, Task>? OnParticipantJoinedAsync;

	public ParticipantSyncContext(ILogger<ParticipantSyncContext> logger, SyncContext parent)
		: base(logger, parent)
	{
	}

	internal Task InvokeOnParticipantJoinedAsync(ParticipantDto dto, List<SubmittedAnswerDto> submittedAnswerDtos) =>
		OnParticipantJoinedAsync.TryInvokeAsync(dto, submittedAnswerDtos);
}
