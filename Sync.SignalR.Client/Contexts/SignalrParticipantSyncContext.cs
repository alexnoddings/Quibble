using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Common.Dtos;

namespace Quibble.Sync.SignalR.Client.Contexts;

internal class SignalrParticipantSyncContext : BaseSignalrSyncContext, IParticipantSyncContext
{
	public event Func<ParticipantDto, List<SubmittedAnswerDto>, Task>? OnParticipantJoinedAsync;

	public SignalrParticipantSyncContext(ILogger<SignalrParticipantSyncContext> logger, HubConnection hubConnection)
		: base(logger, hubConnection)
	{
		Bind(e => e.OnParticipantJoinedAsync, () => OnParticipantJoinedAsync);
	}
}
