using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Core.Contexts;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;
using Quibble.Shared.Sync.SignalR;

namespace Quibble.Client.Sync.SignalR.Contexts;

internal class SignalrQuestionSyncContext : BaseSignalrSyncContext, IQuestionSyncContext
{
    public event Func<QuestionDto, Task>? OnAddedAsync;
    public event Func<QuestionDto, List<SubmittedAnswerDto>, Task>? OnRevealedAsync;
    public event Func<Guid, string, Task>? OnTextUpdatedAsync;
    public event Func<Guid, string, Task>? OnAnswerUpdatedAsync;
    public event Func<Guid, decimal, Task>? OnPointsUpdatedAsync;
    public event Func<Guid, QuestionState, Task>? OnStateUpdatedAsync;
    public event Func<Guid, int, Task>? OnOrderUpdatedAsync;
    public event Func<Guid, Task>? OnDeletedAsync;

    public SignalrQuestionSyncContext(ILogger<SignalrQuestionSyncContext> logger, HubConnection hubConnection)
        : base(logger, hubConnection)
    {
        Bind(e => e.OnQuestionAddedAsync, () => OnAddedAsync);
        Bind(e => e.OnQuestionRevealedAsync, () => OnRevealedAsync);
        Bind(e => e.OnQuestionTextUpdatedAsync, () => OnTextUpdatedAsync);
        Bind(e => e.OnQuestionAnswerUpdatedAsync, () => OnAnswerUpdatedAsync);
        Bind(e => e.OnQuestionPointsUpdatedAsync, () => OnPointsUpdatedAsync);
        Bind(e => e.OnQuestionStateUpdatedAsync, () => OnStateUpdatedAsync);
        Bind(e => e.OnQuestionOrderUpdatedAsync, () => OnOrderUpdatedAsync);
        Bind(e => e.OnQuestionDeletedAsync, () => OnDeletedAsync);
    }

    public Task AddQuestionAsync(Guid roundId) =>
        HubConnection.InvokeAsync(SignalrEndpoints.CreateQuestion, roundId);

    public Task UpdateTextAsync(Guid id, string newText) =>
        HubConnection.InvokeAsync(SignalrEndpoints.UpdateQuestionText, id, newText);

    public Task UpdateAnswerAsync(Guid id, string newAnswer) =>
        HubConnection.InvokeAsync(SignalrEndpoints.UpdateQuestionAnswer, id, newAnswer);

    public Task UpdatePointsAsync(Guid id, decimal newPoints) =>
        HubConnection.InvokeAsync(SignalrEndpoints.UpdateQuestionPoints, id, newPoints);

    public Task UpdateStateAsync(Guid id, QuestionState newState) =>
        HubConnection.InvokeAsync(SignalrEndpoints.UpdateQuestionState, id, newState);

    public Task DeleteAsync(Guid id) =>
        HubConnection.InvokeAsync(SignalrEndpoints.DeleteQuestion, id);
}
