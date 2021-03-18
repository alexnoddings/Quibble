using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorIdentityBase.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorIdentityBase.Client.Pages
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
