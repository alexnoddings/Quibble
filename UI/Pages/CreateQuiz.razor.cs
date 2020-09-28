using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Services;

namespace Quibble.UI.Pages
{
    [Authorize]
    [Route("/quiz/create")]
    public partial class CreateQuiz
    {
        private string QuizTitle { get; set; } = string.Empty;

        [Inject]
        private IQuizService QuizService { get; init; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; init; } = default!;

        private async Task CreateQuizAsync()
        {
            if (string.IsNullOrEmpty(QuizTitle)) return;

            Guid quizId = await QuizService.CreateAsync(QuizTitle);
            NavigationManager.NavigateTo(GetQuiz.FormatRoute(quizId));
        }

        public static string FormatRoute() => $"/quiz/create";
    }
}