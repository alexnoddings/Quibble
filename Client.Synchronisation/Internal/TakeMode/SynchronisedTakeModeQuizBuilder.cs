using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Internal.TakeMode
{
    internal class SynchronisedTakeModeQuizBuilder
    {
        public ILogger<SignalrSynchronisedEntity>? LoggerInstance { get; set; }
        private HubConnection? HubConnection { get; set; }
        public QuizDto? Quiz { get; set; }
        public List<ParticipantDto>? Participants { get; set; }
        public List<RoundDto>? Rounds { get; set; }
        public List<QuestionDto>? Questions { get; set; }
        public List<SubmittedAnswerDto>? SubmittedAnswers { get; set; }

        public SynchronisedTakeModeQuiz Build()
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

            var synchronisedQuiz = new SynchronisedTakeModeQuiz(LoggerInstance, HubConnection, Quiz);
            foreach (var participant in Participants)
            {
                var synchronisedParticipant = new SynchronisedTakeModeParticipant(LoggerInstance, HubConnection, participant, synchronisedQuiz);
                synchronisedQuiz.SyncedParticipants.Add(synchronisedParticipant);
            }

            foreach (var round in Rounds)
            {
                var synchronisedRound = new SynchronisedTakeModeRound(LoggerInstance, HubConnection, round, synchronisedQuiz);
                foreach (var question in Questions.Where(q => q.RoundId == round.Id))
                {
                    var synchronisedQuestion = new SynchronisedTakeModeQuestion(LoggerInstance, HubConnection, question, synchronisedRound);
                    foreach (var submittedAnswer in SubmittedAnswers.Where(a => a.QuestionId == question.Id))
                    {
                        var synchronisedSubmittedAnswer = new SynchronisedTakeModeSubmittedAnswer(LoggerInstance, HubConnection, submittedAnswer, synchronisedQuestion);
                        synchronisedQuestion.SyncedSubmittedAnswer = synchronisedSubmittedAnswer;
                    }
                    synchronisedRound.SyncedQuestions.Add(synchronisedQuestion);
                }
                synchronisedQuiz.SyncedRounds.Add(synchronisedRound);
            }

            return synchronisedQuiz;
        }

        public SynchronisedTakeModeQuizBuilder WithLoggerInstance(ILogger<SignalrSynchronisedEntity> loggerInstance)
        {
            LoggerInstance = loggerInstance;
            return this;
        }

        public SynchronisedTakeModeQuizBuilder WithHubConnection(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            return this;
        }

        public SynchronisedTakeModeQuizBuilder WithQuiz(QuizDto quiz)
        {
            Quiz = quiz;
            return this;
        }

        public SynchronisedTakeModeQuizBuilder WithParticipants(List<ParticipantDto> participants)
        {
            Participants = participants;
            return this;
        }

        public SynchronisedTakeModeQuizBuilder WithRounds(List<RoundDto> rounds)
        {
            Rounds = rounds;
            return this;
        }

        public SynchronisedTakeModeQuizBuilder WithQuestions(List<QuestionDto> questions)
        {
            Questions = questions;
            return this;
        }

        public SynchronisedTakeModeQuizBuilder WithSubmittedAnswers(List<SubmittedAnswerDto> submittedAnswers)
        {
            SubmittedAnswers = submittedAnswers;
            return this;
        }
    }
}
