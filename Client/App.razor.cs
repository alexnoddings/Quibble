using Microsoft.AspNetCore.Components;
using Quibble.Client.Services.Themeing;

namespace Quibble.Client;

public sealed partial class App : IDisposable
{
    [Inject]
    private ThemeService ThemeService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        await ThemeService.InitialiseAsync();
        ThemeService.ThemeUpdated += OnThemeUpdatedAsync;
    }

    private Task OnThemeUpdatedAsync() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        ThemeService.ThemeUpdated -= OnThemeUpdatedAsync;
    }
}
