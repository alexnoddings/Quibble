using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Shared.Models;

namespace Quibble.Client.Sync.Internal.HostMode
{
    internal class SynchronisedHostModeQuizBuilder
    {
        public ILogger<SynchronisedEntity>? LoggerInstance { get; set; }
        private HubConnection? HubConnection { get; set; }
        public QuizDto? Quiz { get; set; }
        public List<ParticipantDto>? Participants { get; set; }
        public List<RoundDto>? Rounds { get; set; }
        public List<QuestionDto>? Questions { get; set; }
        public List<SubmittedAnswerDto>? SubmittedAnswers { get; set; }

        public SynchronisedHostModeQuiz Build()
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

            var synchronisedQuiz = new SynchronisedHostModeQuiz(LoggerInstance, HubConnection, Quiz);
            Dictionary<Guid, SynchronisedHostModeParticipant> synchronisedParticipants = new();
            foreach (var participant in Participants)
            {
                var synchronisedParticipant = new SynchronisedHostModeParticipant(LoggerInstance, HubConnection, participant, synchronisedQuiz);
                synchronisedParticipants.Add(synchronisedParticipant.Id, synchronisedParticipant);
                synchronisedQuiz.SyncedParticipants.Add(synchronisedParticipant);
            }

            foreach (var round in Rounds)
            {
                var synchronisedRound = new SynchronisedHostModeRound(LoggerInstance, HubConnection, round, synchronisedQuiz);
                foreach (var question in Questions.Where(q => q.RoundId == round.Id))
                {
                    var synchronisedQuestion = new SynchronisedHostModeQuestion(LoggerInstance, HubConnection, question, synchronisedRound);
                    foreach (var submittedAnswer in SubmittedAnswers.Where(a => a.QuestionId == question.Id))
                    {
                        var synchronisedParticipant = synchronisedParticipants[submittedAnswer.ParticipantId];
                        var synchronisedSubmittedAnswer = new SynchronisedHostModeSubmittedAnswer(LoggerInstance, HubConnection, submittedAnswer, synchronisedQuestion, synchronisedParticipant);
                        synchronisedQuestion.SyncedAnswers.Add(synchronisedSubmittedAnswer);
                        synchronisedParticipant.SyncedAnswers.Add(synchronisedSubmittedAnswer);
                    }
                    synchronisedRound.SyncedQuestions.Add(synchronisedQuestion);
                }
                synchronisedQuiz.SyncedRounds.Add(synchronisedRound);
            }

            return synchronisedQuiz;
        }

        public SynchronisedHostModeQuizBuilder WithLoggerInstance(ILogger<SynchronisedEntity> loggerInstance)
        {
            LoggerInstance = loggerInstance;
            return this;
        }

        public SynchronisedHostModeQuizBuilder WithHubConnection(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
            return this;
        }

        public SynchronisedHostModeQuizBuilder WithQuiz(QuizDto quiz)
        {
            Quiz = quiz;
            return this;
        }

        public SynchronisedHostModeQuizBuilder WithParticipants(List<ParticipantDto> participants)
        {
            Participants = participants;
            return this;
        }

        public SynchronisedHostModeQuizBuilder WithRounds(List<RoundDto> rounds)
        {
            Rounds = rounds;
            return this;
        }

        public SynchronisedHostModeQuizBuilder WithQuestions(List<QuestionDto> questions)
        {
            Questions = questions;
            return this;
        }

        public SynchronisedHostModeQuizBuilder WithSubmittedAnswers(List<SubmittedAnswerDto> submittedAnswers)
        {
            SubmittedAnswers = submittedAnswers;
            return this;
        }
    }
}
