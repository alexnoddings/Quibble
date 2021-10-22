using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Quibble.Shared.Models;

namespace Quibble.Client.Components;

public partial class SiteStatsDisplay
{
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = default!;

    private SiteStats? SiteStats { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var httpClient = HttpClientFactory.CreateClient("QuizApi");
        SiteStats = await httpClient.GetFromJsonAsync<SiteStats>("stats");
    }
}
