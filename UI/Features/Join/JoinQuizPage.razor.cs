using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Quibble.UI.Features.Join
{
    [Authorize]
    [Route("/quiz/join")]
    public partial class JoinQuizPage
    {
        [Inject]
        private NavigationManager NavigationManager { get; init; } = default!;

        private string QuizId { get; set; } = string.Empty;

        private Task JoinAsync()
        {
            var id = Guid.Parse(QuizId);
            NavigationManager.NavigateTo(JoinDirectPage.FormatRoute(id));
            return Task.CompletedTask;
        }

        public static string FormatRoute() => $"/quiz/join";
    }
}
