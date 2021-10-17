using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Contexts;

namespace Quibble.Client.Sync.SignalR.Contexts
{
    internal class SignalrSyncContext : ISyncContext, IAsyncDisposable
    {
        private HubConnection HubConnection { get; }
        private bool IsDisposed { get; set; }

        private SignalrQuizSyncContext QuizContext { get; }
        public IQuizSyncContext Quizzes => QuizContext;

        private SignalrRoundSyncContext RoundContext { get; }
        public IRoundSyncContext Rounds => RoundContext;

        private SignalrQuestionSyncContext QuestionContext { get; }
        public IQuestionSyncContext Questions => QuestionContext;

        private SignalrSubmittedAnswerSyncContext SubmittedAnswerContext { get; }
        public ISubmittedAnswerSyncContext SubmittedAnswers => SubmittedAnswerContext;

        private SignalrParticipantSyncContext ParticipantContext { get; }
        public IParticipantSyncContext Participants => ParticipantContext;

        public SignalrSyncContext(ILoggerFactory loggerFactory, HubConnection hubConnection)
        {
            HubConnection = hubConnection;

            QuizContext = new SignalrQuizSyncContext(loggerFactory.CreateLogger<SignalrQuizSyncContext>(), hubConnection);
            RoundContext = new SignalrRoundSyncContext(loggerFactory.CreateLogger<SignalrRoundSyncContext>(), hubConnection);
            QuestionContext = new SignalrQuestionSyncContext(loggerFactory.CreateLogger<SignalrQuestionSyncContext>(), hubConnection);
            SubmittedAnswerContext = new SignalrSubmittedAnswerSyncContext(loggerFactory.CreateLogger<SignalrSubmittedAnswerSyncContext>(), hubConnection);
            ParticipantContext = new SignalrParticipantSyncContext(loggerFactory.CreateLogger<SignalrParticipantSyncContext>(), hubConnection);
        }

        public async ValueTask DisposeAsync()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;

            QuizContext.Dispose();
            RoundContext.Dispose();
            QuestionContext.Dispose();
            SubmittedAnswerContext.Dispose();
            ParticipantContext.Dispose();

            await HubConnection.DisposeAsync();
        }
    }
}
