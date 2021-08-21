using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities.TakeMode
{
    internal class SignalrSyncedTakeModeQuizBuilder
    {
        public ILogger<SignalrSyncedEntity>? LoggerInstance { get; set; }
        private HubConnection? HubConnection { get; set; }
        public QuizDto? Quiz { get; set; }
        public List<ParticipantDto>? Participants { get; set; }
        public List<RoundDto>? Rounds { get; set; }
        public List<QuestionDto>? Questions { get; set; }
        public List<SubmittedAnswerDto>? SubmittedAnswers { get; set; }

        public SignalrSyncedTakeModeQuiz Build()
        {
            if (LoggerInstance is null)
                throw new InvalidOperationException($"{nameof(LoggerInstance)} must be set.");
            if (HubConnection is null)
                throw new InvalidOperationException($"{nameof(HubConnection)} must be set.");
            if (Quiz is null)
                throw new InvalidOperationException($"{nameof(Quiz)} must be set.");
            if (Participants is null)
                throw new InvalidOperationException($"{nameof(Participants)} must be set.");
            if (Rounds is null)
                throw new InvalidOperationException($"{nameof(Rounds)} must be set.");
            if (Questions is null)
                throw new InvalidOperationException($"{nameof(Questions)} must be set.");
            if (SubmittedAnswers is null)
                throw new InvalidOperationException($"{nameof(SubmittedAnswers)} must be set.");

            var synchronisedQuiz = new SignalrSyncedTakeModeQuiz(LoggerInstance, HubConnection, Quiz);
            foreach (var participant in Participants)
            {
                var synchronisedParticipant = new SignalrSyncedTakeModeParticipant(LoggerInstance, HubConnection, participant, synchronisedQuiz);
                synchronisedQuiz.SyncedParticipants.Add(synchronisedParticipant);
            }

            foreach (var round in Rounds)
            {
                var synchronisedRound = new SignalrSyncedTakeModeRound(LoggerInstance, HubConnection, round, synchronisedQuiz);
                foreach (var question in Questions.Where(q => q.RoundId == round.Id))
                {
                    var synchronisedQuestion = new SignalrSyncedTakeModeQuestion(LoggerInstance, HubConnection, question, synchronisedRound);
                    foreach (var submittedAnswer in SubmittedAnswers.Where(a => a.QuestionId == question.Id))
                    {
                        var synchronisedSubmittedAnswer = new SignalrSyncedTakeModeSubmittedAnswer(LoggerInstance, HubConnection, submittedAnswer, synchronisedQuestion);
                        synchronisedQuestion.SyncedSubmittedAnswer = synchronisedSubmittedAnswer;
                    }
                    synchronisedRound.SyncedQuestions.Add(synchronisedQuestion);
                }
                synchronisedQuiz.SyncedRounds.Add(synchronisedRound);
            }

            return synchronisedQuiz;
        }

        public SignalrSyncedTakeModeQuizBuilder WithLoggerInstance(ILogger<SignalrSyncedEntity> loggerInstance)
        {
            LoggerInstance = loggerInstance;
            return this;
        }

        public SignalrSyncedTakeModeQuizBuilder WithHubConnection(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            return this;
        }

        public SignalrSyncedTakeModeQuizBuilder WithQuiz(QuizDto quiz)
        {
            Quiz = quiz;
            return this;
        }

        public SignalrSyncedTakeModeQuizBuilder WithParticipants(List<ParticipantDto> participants)
        {
            Participants = participants;
            return this;
        }

        public SignalrSyncedTakeModeQuizBuilder WithRounds(List<RoundDto> rounds)
        {
            Rounds = rounds;
            return this;
        }

        public SignalrSyncedTakeModeQuizBuilder WithQuestions(List<QuestionDto> questions)
        {
            Questions = questions;
            return this;
        }

        public SignalrSyncedTakeModeQuizBuilder WithSubmittedAnswers(List<SubmittedAnswerDto> submittedAnswers)
        {
            SubmittedAnswers = submittedAnswers;
            return this;
        }
    }
}
