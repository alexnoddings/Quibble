using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Shared;

namespace Quibble.Client.Pages
{
    public partial class FetchData
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        private WeatherForecast[] Forecasts { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Forecasts = await HttpClient.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast");
        }
    }
}
