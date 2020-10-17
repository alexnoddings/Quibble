using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Quibble.UI.Features.Quiz.Join
{
    [Authorize]
    [Route("/quiz/join")]
    public partial class JoinQuiz
    {
        [Inject]
        private NavigationManager NavigationManager { get; init; } = default!;

        private string QuizId { get; set; } = string.Empty;

        private Task JoinAsync()
        {
            var id = Guid.Parse(QuizId);
            NavigationManager.NavigateTo(JoinDirect.FormatRoute(id));
            return Task.CompletedTask;
        }
    }
}
