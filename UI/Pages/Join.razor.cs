using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Quibble.UI.Pages
{
    [Authorize]
    [Route("/quiz/join")]
    public partial class Join
    {
        [Inject]
        private NavigationManager NavigationManager { get; init; } = default!;

        private string QuizId { get; set; } = string.Empty;

        private async Task JoinAsync()
        {
            var id = Guid.Parse(QuizId);
            NavigationManager.NavigateTo(JoinDirect.FormatRoute(id));
        }
    }
}
