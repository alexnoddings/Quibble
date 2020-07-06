using System.Threading.Tasks;
using Quibble.Client.Extensions.Grpc;

namespace Quibble.Client.Pages.Quizzes
{
    public partial class CreateQuiz
    {
        private string NewQuizTitle { get; set; } = string.Empty;

        private string? ErrorDetail { get; set; } = null;

        private async Task CreateQuizAsync()
        {
            var reply = await QuizClient
                .CreateAsync(NewQuizTitle)
                .ConfigureAwait(false);

            if (reply.Ok)
            {
                NavigationManager.NavigateTo($"/quiz/{reply.Value.Id}");
                return;
            }

            ErrorDetail = reply.StatusDetail;
        }
    }
}
