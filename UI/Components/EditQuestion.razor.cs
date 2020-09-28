using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Components
{
    public sealed partial class EditQuestion : IDisposable
    {
        [Parameter]
        public SyncedQuestion Question { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (Question == null) throw new ArgumentException(nameof(Question));

            await base.OnInitializedAsync();

            Question.Updated += OnQuestionUpdatedAsync;
        }

        private Task OnQuestionUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            Question.Updated -= OnQuestionUpdatedAsync;
        }
    }
}
