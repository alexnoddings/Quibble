﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Client.Sync.Entities.TakeMode;

namespace Quibble.Client.Pages.Quiz.Take
{
    public sealed partial class TakeRoundView : IDisposable
    {
        [Parameter]
        public ISynchronisedTakeModeRound Round { get; set; } = default!;

        [Parameter]
        public int Index { get; set; }

        private int PreviousIndex { get; set; }

        private int LastStateStamp { get; set; } = 0;

        private List<ISynchronisedTakeModeQuestion> KnownQuestions { get; } = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Round.Updated += OnUpdatedAsync;
            RegisterUpdateEvents();

            LastStateStamp = Round.GetStateStamp();
            PreviousIndex = Index;
        }

        private Task OnUpdatedAsync()
        {
            RegisterUpdateEvents();
            return InvokeAsync(StateHasChanged);
        }

        private void RegisterUpdateEvents()
        {
            foreach (var question in Round.Questions)
            {
                if (KnownQuestions.Contains(question))
                    continue;
                
                question.Updated += OnUpdatedAsync;
                if (question.SubmittedAnswer is not null)
                    question.SubmittedAnswer.Updated += OnUpdatedAsync;

                KnownQuestions.Add(question);
            }
        }

        protected override bool ShouldRender()
        {
            if (Index != PreviousIndex)
            {
                PreviousIndex = Index;
                return true;
            }

            var currentStateStamp = Round.GetStateStamp();
            if (currentStateStamp == LastStateStamp)
                return false;
            
            LastStateStamp = currentStateStamp;
            return true;
        }

        public void Dispose()
        {
            Round.Updated -= OnUpdatedAsync;
            foreach (var question in Round.Questions)
            {
                question.Updated -= OnUpdatedAsync;
                if (question.SubmittedAnswer is not null)
                    question.SubmittedAnswer.Updated -= OnUpdatedAsync;
            }
        }
    }
}
