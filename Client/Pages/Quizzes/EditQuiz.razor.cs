using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Common.Protos;
using Quibble.Client.Extensions.Grpc;
using Quibble.Client.Extensions.SignalR;
using Quibble.Client.Grpc;
using Quibble.Common.SignalR;

namespace Quibble.Client.Pages.Quizzes
{
    public partial class EditQuiz : IAsyncDisposable
    {
        [Parameter]
        public string Id { get; set; } = string.Empty;

        private QuizInfo? QuizInfo { get; set; }

        private string? ErrorDetail { get; set; }

        private HubConnection? HubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            GrpcReply<QuizInfo> quizInfoReply = await QuizClient.GetAsync(Id).ConfigureAwait(false);
            if (quizInfoReply.Ok)
                QuizInfo = quizInfoReply.Value;
            else
                ErrorDetail = quizInfoReply.StatusDetail;

            HubConnection = new HubConnectionBuilder()
                .WithAuthenticatedRelativeUrl(NavigationManager, "/hubs/quiz", AccessTokenProvider)
                .Build();

            HubConnection.On<string>(nameof(IQuizHub.OnQuizTitleUpdated), OnQuizTitleUpdatedAsync);

            await HubConnection.StartAsync().ConfigureAwait(false);
        }

        private async Task UpdateQuizTitleAsync()
        {
            if (QuizInfo == null) return;

            var newTitle = QuizInfo.Title;
            if (newTitle.Length < 3) return;

            var reply = await QuizClient.UpdateTitleAsync(Id, newTitle).ConfigureAwait(false);
            if (!reply.Ok)
                ErrorDetail = reply.StatusDetail;
        }

        private Task OnQuizTitleUpdatedAsync(string newTitle)
        {
            if (QuizInfo != null)
                QuizInfo.Title = newTitle;

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
