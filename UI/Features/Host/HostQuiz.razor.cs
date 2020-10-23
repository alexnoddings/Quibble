﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Core.Entities;
using Quibble.UI.Core.Entities;
using Quibble.UI.Features.Join;

namespace Quibble.UI.Features.Host
{
    public sealed partial class HostQuiz : IDisposable
    {
        [CascadingParameter]
        public SyncedQuiz Quiz { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; init; } = default!;

        private IEnumerable<(SyncedParticipant, int)> ParticipantsWithScore =>
            Quiz
            .Participants
            .Select(p =>
                (p,
                Quiz
                .Rounds
                .SelectMany(r => r.Questions)
                .SelectMany(q => q.Answers)
                .Where(a => a.ParticipantId == p.Id)
                .Count(a => a.Mark == AnswerMark.Right))
            )
            .OrderByDescending(tuple => tuple.Item2);

        private int SelectedRoundNumber { get; set; } = 0;
        private int SelectedQuestionNumber { get; set; } = 0;

        private SyncedRound SelectedRound => Quiz.Rounds[SelectedRoundNumber];
        private SyncedQuestion SelectedQuestion => SelectedRound.Questions[SelectedQuestionNumber];

        protected override async Task OnInitializedAsync()
        {
            if (Quiz == null) throw new ArgumentException(nameof(Quiz));
            if (NavigationManager == null) throw new ArgumentException(nameof(NavigationManager));

            await base.OnInitializedAsync();

            Quiz.Updated += OnQuizUpdatedAsync;
        }

        private Task OnQuizUpdatedAsync() => InvokeAsync(StateHasChanged);

        private void MoveToPreviousQuestion()
        {
            if (SelectedQuestionNumber > 0)
            {
                SelectedQuestionNumber--;
            }
            else if (SelectedRoundNumber > 0)
            {
                SelectedRoundNumber--;
                SelectedQuestionNumber = SelectedRound.Questions.Count - 1;
            }
        }

        private void MoveToNextQuestion()
        {
            if (SelectedQuestionNumber < SelectedRound.Questions.Count - 1)
            {
                SelectedQuestionNumber++;
            }
            else if (SelectedRoundNumber < Quiz.Rounds.Count - 1)
            {
                SelectedQuestionNumber = 0;
                SelectedRoundNumber++;
            }
        }

        private string GenerateInviteLink()
        {
            var uriBuilder = new UriBuilder(NavigationManager.BaseUri)
            {
                Path = JoinDirectPage.FormatRoute(Quiz.Id)
            };
            return uriBuilder.Uri.ToString();
        }

        public void Dispose()
        {
            if (Quiz != null)
                Quiz.Updated -= OnQuizUpdatedAsync;
        }
    }
}