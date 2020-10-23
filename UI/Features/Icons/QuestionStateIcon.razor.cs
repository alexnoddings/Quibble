using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Quibble.Core.Entities;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Features.Icons
{
    public sealed partial class QuestionStateIcon : IDisposable
    {
        [Parameter]
        public SyncedQuestion Question { get; set; } = default!;

        private IconName StateIconName =>
            Question.State switch
            {
                QuestionState.Hidden => IconName.EyeSlash,
                QuestionState.Visible => IconName.Eye,
                QuestionState.Locked => IconName.Lock,
                QuestionState.AnswerRevealed => IconName.Check,
                _ => throw new ArgumentException($"Unexpected state {Question.State}")
            };

        protected override async Task OnInitializedAsync()
        {
            if (Question == null) throw new ArgumentException(nameof(Question));

            await base.OnInitializedAsync();

            Question.Updated += OnQuestionUpdatedAsync;
        }

        private Task OnQuestionUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            if (Question != null)
                Question.Updated -= OnQuestionUpdatedAsync;
        }
    }
}
