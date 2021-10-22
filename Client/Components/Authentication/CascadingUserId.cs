using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using System.Security.Claims;

namespace Quibble.Client.Components.Authentication;

public sealed class CascadingUserId : ComponentBase, IDisposable
{
    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private Task<Guid> CurrentUserIdTask { get; set; } = default!;
    private Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    protected override void OnInitialized()
    {
        AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;

        AuthenticationStateTask = AuthenticationStateProvider.GetAuthenticationStateAsync();
        CurrentUserIdTask = GetUserIdAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<Task<Guid>>>(1);
        builder.AddAttribute(2, nameof(CascadingValue<Task<Guid>>.Value), CurrentUserIdTask);
        builder.AddAttribute(3, nameof(CascadingValue<Task<Guid>>.ChildContent), ChildContent);
        builder.CloseComponent();
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> newAuthenticationStateTask)
    {
        _ = InvokeAsync(() =>
        {
            AuthenticationStateTask = newAuthenticationStateTask;
            CurrentUserIdTask = GetUserIdAsync();
            StateHasChanged();
        });
    }

    private async Task<Guid> GetUserIdAsync()
    {
        var authState = await AuthenticationStateTask;
        var idStr = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (idStr is null)
            return Guid.Empty;

        return Guid.Parse(idStr);
    }

    public void Dispose()
    {
        AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
}

