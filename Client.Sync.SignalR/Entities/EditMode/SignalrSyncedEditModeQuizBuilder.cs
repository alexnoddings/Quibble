using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities.EditMode
{
    internal class SignalrSyncedEditModeQuizBuilder
    {
        public ILogger<SignalrSyncedEntity>? LoggerInstance { get; set; }
        public HubConnection? HubConnection { get; set; }
        public QuizDto? Quiz { get; set; }
        public List<RoundDto>? Rounds { get; set; }
        public List<QuestionDto>? Questions { get; set; }

        public SignalrSyncedEditModeQuiz Build()
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

            var synchronisedQuiz = new SignalrSyncedEditModeQuiz(LoggerInstance, HubConnection, Quiz);
            foreach (var round in Rounds)
            {
                var synchronisedRound = new SignalrSyncedEditModeRound(LoggerInstance, HubConnection, round, synchronisedQuiz);
                foreach (var question in Questions.Where(q => q.RoundId == round.Id))
                {
                    var synchronisedQuestion = new SignalrSyncedEditModeQuestion(LoggerInstance, HubConnection, question, synchronisedRound);
                    synchronisedRound.SyncedQuestions.Add(synchronisedQuestion);
                }
                synchronisedQuiz.SyncedRounds.Add(synchronisedRound);
            }

            return synchronisedQuiz;
        }

        public SignalrSyncedEditModeQuizBuilder WithLoggerInstance(ILogger<SignalrSyncedEntity> loggerInstance)
        {
            LoggerInstance = loggerInstance;
            return this;
        }

        public SignalrSyncedEditModeQuizBuilder WithHubConnection(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            return this;
        }

        public SignalrSyncedEditModeQuizBuilder WithQuiz(QuizDto quiz)
        {
            Quiz = quiz;
            return this;
        }

        public SignalrSyncedEditModeQuizBuilder WithRounds(List<RoundDto> rounds)
        {
            Rounds = rounds;
            return this;
        }

        public SignalrSyncedEditModeQuizBuilder WithQuestions(List<QuestionDto> questions)
        {
            Questions = questions;
            return this;
        }
    }
}
