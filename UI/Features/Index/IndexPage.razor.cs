using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Quibble.UI.Core.Services.Data;
using Quibble.UI.Operations;

namespace Quibble.UI.Features.Index
{
    [Route("/")]
    public sealed partial class IndexPage
    {
        [CascadingParameter] 
        public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

        [Inject]
        private IQuizService QuizService { get; init; } = default!;

        private UiOperation<List<(string, Guid)>> OwnedQuizzesOperation { get; } = UiOperation<List<(string, Guid)>>.Empty();
        private UiOperation<List<(string, Guid)>> ParticipatedQuizzesOperation { get; } = UiOperation<List<(string, Guid)>>.Empty();

        protected override async Task OnInitializedAsync()
        {
            if (AuthenticationStateTask == null) throw new ArgumentException(nameof(AuthenticationStateTask));

            await base.OnInitializedAsync();

            AuthenticationState? authState = await AuthenticationStateTask;
            if (authState?.User?.Identity?.IsAuthenticated == true)
            {
                OwnedQuizzesOperation.Set(QuizService.GetOwnedQuizzesAsync());
                await OwnedQuizzesOperation;
                ParticipatedQuizzesOperation.Set(QuizService.GetParticipatedQuizzesAsync());
                await ParticipatedQuizzesOperation;
            }
        }
    }
}
