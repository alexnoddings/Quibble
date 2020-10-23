using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Features.Edit
{
    public sealed partial class EditRound : IDisposable
    {
        [Parameter]
        public SyncedRound Round { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (Round == null) throw new ArgumentException(nameof(Round));

            await base.OnInitializedAsync();

            Round.Updated += OnRoundUpdatedAsync;
        }

        private Task OnRoundUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            Round.Updated -= OnRoundUpdatedAsync;
        }
    }
}
