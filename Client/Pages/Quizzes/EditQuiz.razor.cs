using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Common.Protos;
using Quibble.Client.Extensions.Grpc;
using Quibble.Client.Extensions.SignalR;
using Quibble.Client.Grpc;
using Quibble.Client.Hubs;

namespace Quibble.Client.Pages.Quizzes
{
    public partial class EditQuiz : IAsyncDisposable
    {
        [Parameter]
        public string Id { get; set; } = string.Empty;

        private QuizFull? QuizFull { get; set; }

        private string? ErrorDetail { get; set; }

        private QuizHubConnection? HubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            GrpcReply<QuizFull> quizFullReply = await QuizClient.GetFullAsync(Id).ConfigureAwait(false);
            if (quizFullReply.Ok)
                QuizFull = quizFullReply.Value;
            else
                ErrorDetail = quizFullReply.StatusDetail;

            HubConnection = new HubConnectionBuilder()
                .WithAuthenticatedRelativeUrl(NavigationManager, "/hubs/quiz", AccessTokenProvider)
                .Build()
                .AsQuizHubConnection();

            HubConnection.OnQuizUpdated(OnQuizUpdatedAsync);
            HubConnection.OnQuizDeleted(OnQuizDeletedAsync);

            HubConnection.OnRoundCreated(OnRoundCreatedAsync);
            HubConnection.OnRoundUpdated(OnRoundUpdatedAsync);
            HubConnection.OnRoundDeleted(OnRoundDeletedAsync);

            HubConnection.OnQuestionCreated(OnQuestionCreatedAsync);
            HubConnection.OnQuestionUpdated(OnQuestionUpdatedAsync);
            HubConnection.OnQuestionDeleted(OnQuestionDeletedAsync);

            await HubConnection.StartAsync().ConfigureAwait(false);

            await HubConnection.RegisterToQuizUpdatesAsync(Id).ConfigureAwait(false);
        }

        #region Actions
        private async Task UpdateQuizTitleAsync()
        {
            if (QuizFull == null) return;

            var newTitle = QuizFull.Info.Title;

            var reply = await QuizClient.UpdateTitleAsync(Id, newTitle).ConfigureAwait(false);
            if (!reply.Ok)
                ErrorDetail = reply.StatusDetail;
        }

        private async Task DeleteQuizAsync()
        {
            if (QuizFull == null) return;

            var reply = await QuizClient.DeleteAsync(QuizFull.Info.Id).ConfigureAwait(false);
            if (!reply.Ok)
                ErrorDetail = reply.StatusDetail;
        }

        private async Task CreateRoundAsync(int count)
        {
            if (QuizFull == null) return;
            if (count <= 0) return;

            for (var i = 0; i < count; i++)
            {
                var reply = await RoundClient.CreateAsync(QuizFull.Info.Id).ConfigureAwait(false);
                if (reply.Ok) continue;
                
                ErrorDetail = reply.StatusDetail;
                break;
            }
        }

        private async Task UpdateRoundTitleAsync(RoundFull round)
        {
            if (QuizFull == null) return;

            var newTitle = round.Info.Title;
            var reply = await RoundClient.UpdateTitleAsync(round.Info.Id, newTitle).ConfigureAwait(false);
            if (!reply.Ok)
                ErrorDetail = reply.StatusDetail;
        }

        private async Task DeleteRoundAsync(RoundFull round)
        {
            if (QuizFull == null) return;

            var reply = await RoundClient.DeleteAsync(round.Info.Id).ConfigureAwait(false);
            if (!reply.Ok)
                ErrorDetail = reply.StatusDetail;
        }

        private async Task CreateQuestionAsync(RoundFull round, int count)
        {
            if (QuizFull == null) return;
            if (count <= 0) return;

            for (var i = 0; i < count; i++)
            {
                var reply = await QuestionClient.CreateAsync(round.Info.Id).ConfigureAwait(false);
                if (reply.Ok) continue;
                
                ErrorDetail = reply.StatusDetail;
                break;
            }
        }

        private Task UpdateQuestionBodyAsync(QuestionInfo question) => UpdateQuestionAsync(question);

        private Task UpdateQuestionAnswerAsync(QuestionInfo question) => UpdateQuestionAsync(question);

        private async Task UpdateQuestionAsync(QuestionInfo question)
        {
            if (QuizFull == null) return;

            var reply = await QuestionClient.UpdateAsync(question.Id, question.Body, question.Answer).ConfigureAwait(false);
            if (!reply.Ok)
                ErrorDetail = reply.StatusDetail;
        }

        private async Task DeleteQuestionAsync(QuestionInfo question)
        {
            if (QuizFull == null) return;

            var reply = await QuestionClient.DeleteAsync(question.Id).ConfigureAwait(false);
            if (!reply.Ok)
                ErrorDetail = reply.StatusDetail;
        }
        #endregion

        #region Events
        private Task OnQuizUpdatedAsync(QuizInfo quizInfo)
        {
            if (QuizFull == null) return Task.CompletedTask;

            QuizFull.Info = quizInfo;

            return InvokeAsync(StateHasChanged);
        }

        private Task OnQuizDeletedAsync(string quizId)
        {
            if (QuizFull == null) return Task.CompletedTask;

            NavigationManager.NavigateTo("/");

            return InvokeAsync(StateHasChanged);
        }

        private Task OnRoundCreatedAsync(RoundInfo roundInfo)
        {
            if (QuizFull == null) return Task.CompletedTask;

            var roundFull = new RoundFull {Info = roundInfo};
            QuizFull.Rounds.Add(roundFull);

            return StateHasChangedAsync();
        }

        private Task OnRoundUpdatedAsync(RoundInfo roundInfo)
        {
            if (QuizFull == null) return Task.CompletedTask;

            var roundFull = QuizFull.Rounds.FirstOrDefault(r => r.Info.Id == roundInfo.Id);
            if (roundFull == null)
            {
                roundFull = new RoundFull {Info = roundInfo};
                QuizFull.Rounds.Add(roundFull);
            }
            else
            {
                roundFull.Info = roundInfo;
            }

            return StateHasChangedAsync();
        }

        private Task OnRoundDeletedAsync(string roundId)
        {
            if (QuizFull == null) return Task.CompletedTask;

            var roundFull = QuizFull.Rounds.FirstOrDefault(r => r.Info.Id == roundId);
            if (roundFull != null)
                QuizFull.Rounds.Remove(roundFull);

            return StateHasChangedAsync();
        }

        private Task OnQuestionCreatedAsync(QuestionInfo questionInfo)
        {
            if (QuizFull == null) return Task.CompletedTask;

            var roundFull = QuizFull.Rounds.FirstOrDefault(r => r.Info.Id == questionInfo.RoundId);
            if (roundFull == null) return Task.CompletedTask;

            roundFull.Questions.Add(questionInfo);

            return StateHasChangedAsync();
        }

        private Task OnQuestionUpdatedAsync(QuestionInfo questionInfo)
        {
            if (QuizFull == null) return Task.CompletedTask;

            var roundFull = QuizFull.Rounds.FirstOrDefault(r => r.Info.Id == questionInfo.RoundId);
            if (roundFull == null) return Task.CompletedTask;

            var existing = roundFull.Questions.FirstOrDefault(q => q.Id == questionInfo.Id);
            int index = roundFull.Questions.IndexOf(existing);
            roundFull.Questions.RemoveAt(index);
            roundFull.Questions.Insert(index, questionInfo);

            Console.WriteLine(string.Join("", Enumerable.Range(0, 10).Select(_ => $"here{_}\n")));

            Console.WriteLine($"indexof: {index}");
            Console.WriteLine($"len: {roundFull.Questions.Count}");

            Console.WriteLine(string.Join("", Enumerable.Range(0, 10).Select(_ => $"here{_}\n")));

            return StateHasChangedAsync();
        }

        private Task OnQuestionDeletedAsync(string roundId, string questionId)
        {
            if (QuizFull == null) return Task.CompletedTask;

            var roundFull = QuizFull.Rounds.FirstOrDefault(r => r.Info.Id == roundId);
            if (roundFull == null) return Task.CompletedTask;

            var existing = roundFull.Questions.FirstOrDefault(q => q.Id == questionId);
            roundFull.Questions.Remove(existing);

            return StateHasChangedAsync();
        }
        #endregion

        private Task StateHasChangedAsync() => InvokeAsync(StateHasChanged);

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (HubConnection != null)
                await HubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
