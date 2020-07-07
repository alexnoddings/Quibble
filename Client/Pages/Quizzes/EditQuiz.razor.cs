using System;
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

            await HubConnection.StartAsync().ConfigureAwait(false);

            await HubConnection.RegisterToQuizUpdatesAsync(Id).ConfigureAwait(false);
        }

        private async Task UpdateQuizTitleAsync()
        {
            if (QuizFull == null) return;

            var newTitle = QuizFull.Info.Title;
            if (newTitle.Length < 3) return;

            var reply = await QuizClient.UpdateTitleAsync(Id, newTitle).ConfigureAwait(false);
            if (!reply.Ok)
                ErrorDetail = reply.StatusDetail;
        }

        private Task OnQuizUpdatedAsync(string newTitle)
        {
            if (QuizFull != null)
                QuizFull.Info.Title = newTitle;

            return InvokeAsync(StateHasChanged);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (HubConnection != null)
                await HubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
