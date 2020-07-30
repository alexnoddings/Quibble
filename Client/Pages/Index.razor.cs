using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Common.Quizzes;

namespace Quibble.Client.Pages
{
    public partial class Index
    {
        private List<Quiz> OwnedQuizzes { get; } = new List<Quiz>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            await QuizHubConnection.StartAsync().ConfigureAwait(false);

            var quizzes = await QuizHubConnection.GetOwnedAsync().ConfigureAwait(false);
            OwnedQuizzes.Clear();
            OwnedQuizzes.AddRange(quizzes);
        }
    }
}
