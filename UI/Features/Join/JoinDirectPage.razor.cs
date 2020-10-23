using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Services.Data;
using Quibble.UI.Features.GetQuiz;

namespace Quibble.UI.Features.Join
{
    [Authorize]
    [Route("/quiz/join/{id:guid}")]
    public partial class JoinDirectPage
    {
        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        private IParticipantService ParticipantService { get; init; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; init; } = default!;

        public static string FormatRoute(Guid quizId) => $"/quiz/join/{quizId}";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await ParticipantService.JoinAsync(Id);
            NavigationManager.NavigateTo(GetQuizPage.FormatRoute(Id));
        }
    }
}
