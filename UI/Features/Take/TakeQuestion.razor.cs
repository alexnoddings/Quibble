using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Features.Take
{
    public sealed partial class TakeQuestion : IDisposable
    {
        [CascadingParameter]
        public SyncedQuestion Question { get; set; } = default!;

        [CascadingParameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        [CascadingParameter]
        public SyncedParticipant CurrentParticipant { get; set; } = default!;

        private SyncedAnswer CurrentAnswer { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (Question == null) throw new ArgumentException(nameof(Question));

            await base.OnInitializedAsync();

            Question.Updated += OnQuestionUpdatedAsync;

            CurrentAnswer = Question.Answers.First(a => a.ParticipantId == CurrentParticipant.Id);
        }

        private Task OnQuestionUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            if (Question != null)
                Question.Updated -= OnQuestionUpdatedAsync;
        }
    }
}
