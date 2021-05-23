using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Shared.Models;

namespace Quibble.Client.Sync.Internal.EditMode
{
    internal class SynchronisedEditModeQuizBuilder
    {
        public ILogger<SynchronisedEntity>? LoggerInstance { get; set; }
        public HubConnection? HubConnection { get; set; }
        public QuizDto? Quiz { get; set; }
        public List<RoundDto>? Rounds { get; set; }
        public List<QuestionDto>? Questions { get; set; }

        public SynchronisedEditModeQuiz Build()
        {
            if (LoggerInstance is null)
                throw new InvalidOperationException($"{nameof(LoggerInstance)} must be set.");
            if (HubConnection is null)
                throw new InvalidOperationException($"{nameof(HubConnection)} must be set.");
            if (Quiz is null)
                throw new InvalidOperationException($"{nameof(Quiz)} must be set.");
            if (Rounds is null)
                throw new InvalidOperationException($"{nameof(Rounds)} must be set.");
            if (Questions is null)
                throw new InvalidOperationException($"{nameof(Questions)} must be set.");

            var synchronisedQuiz = new SynchronisedEditModeQuiz(LoggerInstance, HubConnection, Quiz);
            foreach (var round in Rounds)
            {
                var synchronisedRound = new SynchronisedEditModeRound(LoggerInstance, HubConnection, round, synchronisedQuiz);
                foreach (var question in Questions.Where(q => q.RoundId == round.Id))
                {
                    var synchronisedQuestion = new SynchronisedEditModeQuestion(LoggerInstance, HubConnection, question, synchronisedRound);
                    synchronisedRound.SyncedQuestions.Add(synchronisedQuestion);
                }
                synchronisedQuiz.SyncedRounds.Add(synchronisedRound);
            }

            return synchronisedQuiz;
        }

        public SynchronisedEditModeQuizBuilder WithLoggerInstance(ILogger<SynchronisedEntity> loggerInstance)
        {
            LoggerInstance = loggerInstance;
            return this;
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
