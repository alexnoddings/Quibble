﻿using Microsoft.AspNetCore.Components;
using Quibble.UI.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Quibble.UI.Features.Quiz.Host
{
    public sealed partial class HostAnswer : IDisposable
    {
        [CascadingParameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        [Parameter]
        public SyncedAnswer Answer { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (Answer == null) throw new ArgumentException(nameof(Quiz));

            await base.OnInitializedAsync();

            Answer.Updated += OnAnswerUpdatedAsync;
        }

        private Task OnAnswerUpdatedAsync() => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            if (Answer != null)
                Answer.Updated -= OnAnswerUpdatedAsync;
        }
    }
}
