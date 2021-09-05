using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Contexts;
using Quibble.Client.Sync.Core;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Entities
{
    internal sealed class SyncedQuiz : SyncedEntity, ISyncedQuiz
    {
        public Guid OwnerId { get; }

        public string Title { get; private set; }
        public QuizState State { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? OpenedAt { get; private set; }

        public bool IsDeleted { get; private set; }

        internal SyncedEntitiesList<SyncedRound> SyncedRounds { get; } = new();
        public ISyncedEntities<ISyncedRound> Rounds => SyncedRounds;

        internal SyncedEntitiesList<SyncedParticipant> SyncedParticipants { get; } = new();
        public ISyncedEntities<ISyncedParticipant> Participants => SyncedParticipants;

        internal SyncedQuiz(ILogger<SyncedEntity> logger, ISyncContext syncContext, QuizDto quiz)
            : base(logger, syncContext)
        {
            Id = quiz.Id;
            OwnerId = quiz.OwnerId;
            Title = quiz.Title;
            State = quiz.State;
            CreatedAt = quiz.CreatedAt;
            OpenedAt = quiz.OpenedAt;

            SyncContext.Quizzes.OnOpenedAsync += OnOpenedAsync;
            SyncContext.Quizzes.OnTitleUpdatedAsync += OnTitleUpdatedAsync;
            SyncContext.Quizzes.OnDeletedAsync += OnDeletedAsync;

            SyncContext.Rounds.OnAddedAsync += OnRoundAddedAsync;
            SyncContext.Rounds.OnDeletedAsync += OnRoundDeletedAsync;

            SyncContext.Participants.OnParticipantJoinedAsync += OnParticipantJoinedAsync;
        }

        public Task AddRoundAsync() =>
            SyncContext.Rounds.AddAsync();

        public Task UpdateTitleAsync(string newTitle) =>
            SyncContext.Quizzes.UpdateTitleAsync(newTitle);

        public Task OpenAsync() =>
            SyncContext.Quizzes.OpenAsync();

        public Task DeleteAsync() =>
            SyncContext.Quizzes.DeleteAsync();

        private Task OnOpenedAsync()
        {
            State = QuizState.Open;
            return OnUpdatedAsync();
        }

        private Task OnTitleUpdatedAsync(string newTitle)
        {
            Title = newTitle;
            return OnUpdatedAsync();
        }

        private Task OnDeletedAsync()
        {
            IsDeleted = true;
            return OnUpdatedAsync();
        }

        private async Task OnRoundAddedAsync(RoundDto roundDto)
        {
            await SyncedRounds.AddAsync(new SyncedRound(Logger, SyncContext, roundDto, this));
            await OnUpdatedAsync();
        }

        private async Task OnRoundDeletedAsync(Guid roundId)
        {
            var round = SyncedRounds.FirstOrDefault(question => question.Id == roundId);
            if (round is null)
                return;

            await SyncedRounds.RemoveAsync(round);
            await OnUpdatedAsync();
        }

        private async Task OnParticipantJoinedAsync(ParticipantDto participant, List<SubmittedAnswerDto> answers)
        {
            var synchronisedParticipant = new SyncedParticipant(Logger, SyncContext, participant, this);
            await SyncedParticipants.AddAsync(synchronisedParticipant);

            foreach (var answer in answers)
            {
                var synchronisedQuestion = SyncedRounds.SelectMany(r => r.SyncedQuestions).First(q => q.Id == answer.QuestionId);
                var synchronisedAnswer = new SyncedSubmittedAnswer(Logger, SyncContext, answer, synchronisedQuestion, synchronisedParticipant);
                await synchronisedQuestion.SyncedAnswers.AddAsync(synchronisedAnswer);
                await synchronisedParticipant.SyncedAnswers.AddAsync(synchronisedAnswer);
            }

            await OnUpdatedAsync();
        }

        public override int GetStateStamp() =>
            StateStamp.ForProperties(Title, State, CreatedAt, OpenedAt, IsDeleted, SyncedRounds, SyncedParticipants);

        public async ValueTask DisposeAsync()
        {
            SyncContext.Quizzes.OnOpenedAsync -= OnOpenedAsync;
            SyncContext.Quizzes.OnTitleUpdatedAsync -= OnTitleUpdatedAsync;
            SyncContext.Quizzes.OnDeletedAsync -= OnDeletedAsync;

            SyncContext.Rounds.OnAddedAsync -= OnRoundAddedAsync;
            SyncContext.Rounds.OnDeletedAsync -= OnRoundDeletedAsync;

            SyncContext.Participants.OnParticipantJoinedAsync -= OnParticipantJoinedAsync;

            await SyncContext.DisposeAsync();
        }
    }
}
