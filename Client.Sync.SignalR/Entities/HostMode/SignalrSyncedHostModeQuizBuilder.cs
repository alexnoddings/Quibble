using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.SignalR.Entities.HostMode
{
    internal class SignalrSyncedHostModeQuizBuilder
    {
        public ILogger<SignalrSyncedEntity>? LoggerInstance { get; set; }
        private HubConnection? HubConnection { get; set; }
        public QuizDto? Quiz { get; set; }
        public List<ParticipantDto>? Participants { get; set; }
        public List<RoundDto>? Rounds { get; set; }
        public List<QuestionDto>? Questions { get; set; }
        public List<SubmittedAnswerDto>? SubmittedAnswers { get; set; }

        public SignalrSyncedHostModeQuiz Build()
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

            var synchronisedQuiz = new SignalrSyncedHostModeQuiz(LoggerInstance, HubConnection, Quiz);
            Dictionary<Guid, SignalrSyncedHostModeParticipant> synchronisedParticipants = new();
            foreach (var participant in Participants)
            {
                var synchronisedParticipant = new SignalrSyncedHostModeParticipant(LoggerInstance, HubConnection, participant, synchronisedQuiz);
                synchronisedParticipants.Add(synchronisedParticipant.Id, synchronisedParticipant);
                synchronisedQuiz.SyncedParticipants.Add(synchronisedParticipant);
            }

            foreach (var round in Rounds)
            {
                var synchronisedRound = new SignalrSyncedHostModeRound(LoggerInstance, HubConnection, round, synchronisedQuiz);
                foreach (var question in Questions.Where(q => q.RoundId == round.Id))
                {
                    var synchronisedQuestion = new SignalrSyncedHostModeQuestion(LoggerInstance, HubConnection, question, synchronisedRound);
                    foreach (var submittedAnswer in SubmittedAnswers.Where(a => a.QuestionId == question.Id))
                    {
                        var synchronisedParticipant = synchronisedParticipants[submittedAnswer.ParticipantId];
                        var synchronisedSubmittedAnswer = new SignalrSyncedHostModeSubmittedAnswer(LoggerInstance, HubConnection, submittedAnswer, synchronisedQuestion, synchronisedParticipant);
                        synchronisedQuestion.SyncedAnswers.Add(synchronisedSubmittedAnswer);
                        synchronisedParticipant.SyncedAnswers.Add(synchronisedSubmittedAnswer);
                    }
                    synchronisedRound.SyncedQuestions.Add(synchronisedQuestion);
                }
                synchronisedQuiz.SyncedRounds.Add(synchronisedRound);
            }

            return synchronisedQuiz;
        }

        public SignalrSyncedHostModeQuizBuilder WithLoggerInstance(ILogger<SignalrSyncedEntity> loggerInstance)
        {
            LoggerInstance = loggerInstance;
            return this;
        }

        public SignalrSyncedHostModeQuizBuilder WithHubConnection(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            return this;
        }

        public SignalrSyncedHostModeQuizBuilder WithQuiz(QuizDto quiz)
        {
            Quiz = quiz;
            return this;
        }

        public SignalrSyncedHostModeQuizBuilder WithParticipants(List<ParticipantDto> participants)
        {
            Participants = participants;
            return this;
        }

        public SignalrSyncedHostModeQuizBuilder WithRounds(List<RoundDto> rounds)
        {
            Rounds = rounds;
            return this;
        }

        public SignalrSyncedHostModeQuizBuilder WithQuestions(List<QuestionDto> questions)
        {
            Questions = questions;
            return this;
        }

        public SignalrSyncedHostModeQuizBuilder WithSubmittedAnswers(List<SubmittedAnswerDto> submittedAnswers)
        {
            SubmittedAnswers = submittedAnswers;
            return this;
        }
    }
}
