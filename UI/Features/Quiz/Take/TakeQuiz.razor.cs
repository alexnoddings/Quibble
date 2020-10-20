using Microsoft.AspNetCore.Components;
using Quibble.Core.Entities;
using Quibble.UI.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Quibble.UI.Features.Quiz.Take
{
    public sealed partial class TakeQuiz : IDisposable
    {
        [CascadingParameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        [CascadingParameter]
        public SyncedParticipant Participant { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (Quiz == null) throw new ArgumentException(nameof(Quiz));

            await base.OnInitializedAsync();

            Quiz.Updated += OnQuizUpdatedAsync;
        }

        private Task OnQuizUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            if (Quiz != null)
                Quiz.Updated -= OnQuizUpdatedAsync;
        }

        private int CalculateScore() =>
            Quiz
            .Rounds
            .SelectMany(r => r.Questions)
            .SelectMany(q => q.Answers)
            .Where(a => a.ParticipantId == Participant.Id)
            .Count(a => a.Mark == AnswerMark.Right);
    }
}
