using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Quibble.Common.Questions;
using Quibble.Common.Quizzes;
using Quibble.Common.Rounds;

namespace Quibble.Client.Pages.Quizzes
{
    public partial class EditQuiz : IAsyncDisposable
    {
        [Parameter]
        public Guid Id { get; set; }

        private QuizFull? Quiz { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            QuizHubConnection.OnQuizUpdated(OnQuizUpdatedAsync);
            QuizHubConnection.OnQuizDeleted(OnQuizDeletedAsync);

            RoundHubConnection.OnRoundCreated(OnRoundCreatedAsync);
            RoundHubConnection.OnRoundUpdated(OnRoundUpdatedAsync);
            RoundHubConnection.OnRoundDeleted(OnRoundDeletedAsync);

            QuestionHubConnection.OnQuestionCreated(OnQuestionCreatedAsync);
            QuestionHubConnection.OnQuestionUpdated(OnQuestionUpdatedAsync);
            QuestionHubConnection.OnQuestionDeleted(OnQuestionDeletedAsync);

            await QuizHubConnection.StartAsync().ConfigureAwait(false);
            await RoundHubConnection.StartAsync().ConfigureAwait(false);
            await QuestionHubConnection.StartAsync().ConfigureAwait(false);

            Quiz = await QuizHubConnection.GetFullAsync(Id).ConfigureAwait(false);

            if (Quiz == null) throw new InvalidOperationException();

            await QuizHubConnection.RegisterForUpdatesAsync(Id).ConfigureAwait(false);
            await RoundHubConnection.RegisterForUpdatesAsync(Id).ConfigureAwait(false);
            await QuestionHubConnection.RegisterForUpdatesAsync(Id).ConfigureAwait(false);
        }

        private Task OnQuizUpdatedAsync(Quiz quiz)
        {
            if (quiz == null || Quiz == null) return Task.CompletedTask;

            Quiz.Title = quiz.Title;
            Quiz.State = quiz.State;

            return StateHasChangedAsync();
        }

        private Task OnQuizDeletedAsync(Guid id)
        {
            NavigationManager.NavigateTo("");

            return Task.CompletedTask;
        }

        private Task OnRoundCreatedAsync(Round round)
        {
            if (round == null || Quiz == null) return Task.CompletedTask;

            Quiz.Rounds.Add(new RoundFull(round));

            return StateHasChangedAsync();
        }

        private Task OnRoundUpdatedAsync(Round round)
        {
            if (round == null || Quiz == null) return Task.CompletedTask;

            var currentRound = Quiz.Rounds.FirstOrDefault(r => r.Id == round.Id);
            if (currentRound == null) return Task.CompletedTask;

            currentRound.Title = round.Title;
            currentRound.State = round.State;

            return StateHasChangedAsync();
        }

        private Task OnRoundDeletedAsync(Guid id)
        {
            if (Quiz == null) return Task.CompletedTask;

            var currentRound = Quiz.Rounds.FirstOrDefault(r => r.Id == id);
            if (currentRound == null) return Task.CompletedTask;
            Quiz.Rounds.Remove(currentRound);

            return StateHasChangedAsync();
        }

        private Task OnQuestionCreatedAsync(Question question)
        {
            if (question == null || Quiz == null) return Task.CompletedTask;

            var parentRound = Quiz.Rounds.FirstOrDefault(r => r.Id == question.RoundId);
            if (parentRound == null) return Task.CompletedTask;
            
            parentRound.Questions.Add(question);

            return StateHasChangedAsync();
        }

        private Task OnQuestionUpdatedAsync(Question question)
        {
            if (question == null || Quiz == null) return Task.CompletedTask;

            var parentRound = Quiz.Rounds.FirstOrDefault(r => r.Id == question.RoundId);
            if (parentRound == null) return Task.CompletedTask;

            var currentQuestion = parentRound.Questions.FirstOrDefault(q => q.Id == question.Id);
            if (currentQuestion == null) return Task.CompletedTask;

            currentQuestion.Body = question.Body;
            currentQuestion.Answer = question.Answer;
            currentQuestion.State = question.State;

            return StateHasChangedAsync();
        }

        private Task OnQuestionDeletedAsync(Guid id)
        {
            if (Quiz == null) return Task.CompletedTask;
            
            var currentQuestion = Quiz.Rounds.SelectMany(r => r.Questions).FirstOrDefault(q => q.Id == id);
            if (currentQuestion == null) return Task.CompletedTask;

            var parentRound = Quiz.Rounds.FirstOrDefault(r => r.Id == currentQuestion.RoundId);
            if (parentRound == null) return Task.CompletedTask;

            parentRound.Questions.Remove(currentQuestion);

            return StateHasChangedAsync();
        }

        private async Task PublishQuizAsync()
        {
            if (Quiz == null) return;

            Quiz.State = QuizState.InProgress;
            var updated = await QuizHubConnection.UpdateAsync(Quiz).ConfigureAwait(false);

            NavigationManager.NavigateTo($"/quiz/{updated.Id}/host");
        }

        private async Task UpdateQuizAsync()
        {
            if (Quiz == null) return;

            await QuizHubConnection.UpdateAsync(Quiz).ConfigureAwait(false);
        }

        private async Task DeleteQuizAsync()
        {
            if (Quiz == null) return;

            await QuizHubConnection.DeleteAsync(Quiz.Id).ConfigureAwait(false);
            NavigationManager.NavigateTo("");
        }

        private async Task CreateRoundAsync()
        {
            if (Quiz == null) return;

            var round = new Round
            {
                QuizId = Quiz.Id
            };

            var created = await RoundHubConnection.CreateAsync(round).ConfigureAwait(false);
            Quiz.Rounds.Add(new RoundFull(created));

            await StateHasChangedAsync().ConfigureAwait(false);
        }

        private Task UpdateRoundAsync(Round round) =>
            RoundHubConnection.UpdateAsync(round);

        private async Task DeleteRoundAsync(RoundFull round)
        {
            if (Quiz == null || round == null) return;

            await RoundHubConnection.DeleteAsync(round.Id).ConfigureAwait(false);
            Quiz.Rounds.Remove(round);

            await StateHasChangedAsync().ConfigureAwait(false);
        }

        private async Task CreateQuestionAsync(RoundFull parentRound)
        {
            if (Quiz == null) return;

            var question = new Question
            {
                RoundId = parentRound.Id
            };

            var created = await QuestionHubConnection.CreateAsync(question).ConfigureAwait(false);
            parentRound.Questions.Add(created);

            await StateHasChangedAsync().ConfigureAwait(false);
        }

        private Task UpdateQuestionAsync(Question question) =>
            QuestionHubConnection.UpdateAsync(question);

        private async Task DeleteQuestionAsync(RoundFull parentRound, Question question)
        {
            if (Quiz == null || question == null) return;

            await QuestionHubConnection.DeleteAsync(question.Id).ConfigureAwait(false);
            parentRound.Questions.Remove(question);

            await StateHasChangedAsync().ConfigureAwait(false);
        }

        private Task StateHasChangedAsync() => InvokeAsync(StateHasChanged);

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (QuizHubConnection != null)
                await QuizHubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
