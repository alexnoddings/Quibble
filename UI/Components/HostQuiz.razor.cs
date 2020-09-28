using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Components
{
    public sealed partial class HostQuiz : IDisposable
    {
        [Parameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (Quiz == null) throw new ArgumentException(nameof(Quiz));

            await base.OnInitializedAsync();

            Quiz.Updated += OnQuizUpdatedAsync;
        }

        private Task OnQuizUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            Quiz.Updated -= OnQuizUpdatedAsync;
        }
    }
}
