using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Shared.Models;

namespace Quibble.Client.Sync.Internal.EditMode
{
    internal class SynchronisedEditModeQuizBuilder
    {
        public HubConnection? HubConnection { get; set; }
        public QuizDto? Quiz { get; set; }
        public List<RoundDto>? Rounds { get; set; }
        public List<QuestionDto>? Questions { get; set; }

        public SynchronisedEditModeQuiz Build()
        {
            if (HubConnection is null)
                throw new InvalidOperationException($"{nameof(HubConnection)} must be set.");
            if (Quiz is null)
                throw new InvalidOperationException($"{nameof(Quiz)} must be set.");
            if (Rounds is null)
                throw new InvalidOperationException($"{nameof(Rounds)} must be set.");
            if (Questions is null)
                throw new InvalidOperationException($"{nameof(Questions)} must be set.");

            var synchronisedQuiz = new SynchronisedEditModeQuiz(HubConnection, Quiz);
            foreach (var round in Rounds)
            {
                var synchronisedRound = new SynchronisedEditModeRound(HubConnection, round);
                foreach (var question in Questions.Where(q => q.RoundId == round.Id))
                {
                    var syncedQuestion = new SynchronisedEditModeQuestion(HubConnection, question);
                    synchronisedRound.SyncedQuestions.Add(syncedQuestion);
                }
                synchronisedQuiz.SyncedRounds.Add(synchronisedRound);
            }

            return synchronisedQuiz;
        }

        public SynchronisedEditModeQuizBuilder WithHubConnection(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            return this;
        }

        public SynchronisedEditModeQuizBuilder WithQuiz(QuizDto quiz)
        {
            Quiz = quiz;
            return this;
        }

        public SynchronisedEditModeQuizBuilder WithRounds(List<RoundDto> rounds)
        {
            Rounds = rounds;
            return this;
        }

        public SynchronisedEditModeQuizBuilder WithQuestions(List<QuestionDto> questions)
        {
            Questions = questions;
            return this;
        }
    }
}
