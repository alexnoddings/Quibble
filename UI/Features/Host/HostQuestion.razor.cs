using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Features.Host
{
    public sealed partial class HostQuestion : IDisposable
    {
        [CascadingParameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        [Parameter]
        public SyncedRound Round { get; set; } = default!;

        [Parameter]
        public SyncedQuestion Question { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (Quiz == null) throw new ArgumentException(nameof(Quiz));
            if (Round == null) throw new ArgumentException(nameof(Round));
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
