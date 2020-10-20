using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Quibble.Core.Exceptions;
using Quibble.Core.Extensions;
using Quibble.UI.Core.Entities;
using Quibble.UI.Core.Services.Data;
using Quibble.UI.Features.Quiz.Join;
using Quibble.UI.Operations;

namespace Quibble.UI.Features.Quiz
{
    [Authorize]
    [Route("/quiz/{id:guid}")]
    public sealed partial class GetQuiz : IDisposable
    {
        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        private ISynchronisedQuizFactory QuizFactory { get; init; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; init; } = default!;

        [CascadingParameter]
        private Task<AuthenticationState> GetAuthenticationState { get; init; } = default!;

        private UiOperation<SyncedQuiz> QuizUiOperation { get; } = UiOperation<SyncedQuiz>.Empty();
        private Guid UserId { get; set; }

        public static string FormatRoute(Guid quizId) => $"/quiz/{quizId}";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            UserId = (await GetAuthenticationState).User.GetId();
            QuizUiOperation.Set(QuizFactory.GetAsync(Id));
            try
            {
                await QuizUiOperation;
            }
            catch (UnauthorisedException e) when (e.Message.Contains("not joined", StringComparison.OrdinalIgnoreCase))
            {
                NavigationManager.NavigateTo(JoinDirect.FormatRoute(Id));
            }

            if (QuizUiOperation.Status == UiOperationStatus.Loaded)
            {
                QuizUiOperation.Result.Published += OnQuizPublishedAsync;
            }
        }

        private Task OnQuizPublishedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            if (QuizUiOperation.Status == UiOperationStatus.Loaded)
            {
                QuizUiOperation.Result.Published -= OnQuizPublishedAsync;
            }
        }
    }
}