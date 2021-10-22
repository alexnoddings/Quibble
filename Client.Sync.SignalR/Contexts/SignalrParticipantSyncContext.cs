using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Core.Contexts;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Contexts;

internal class SignalrParticipantSyncContext : BaseSignalrSyncContext, IParticipantSyncContext
{
    public event Func<ParticipantDto, List<SubmittedAnswerDto>, Task>? OnParticipantJoinedAsync;

    public SignalrParticipantSyncContext(ILogger<SignalrParticipantSyncContext> logger, HubConnection hubConnection)
        : base(logger, hubConnection)
    {
        Bind(e => e.OnParticipantJoinedAsync, () => OnParticipantJoinedAsync);
    }
}
