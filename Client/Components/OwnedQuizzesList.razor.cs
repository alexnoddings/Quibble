using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Client.Extensions.Grpc;
using Quibble.Common.Protos;

namespace Quibble.Client.Components
{
    public partial class OwnedQuizzesList
    {
        private List<QuizInfo> Quizzes { get; } = new List<QuizInfo>();

        private string? Error { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var ownedQuizzesReply = await QuizClient.GetOwnedAsync().ConfigureAwait(false);

            if (ownedQuizzesReply.Ok)
            {
                Quizzes.Clear();
                var infos = ownedQuizzesReply.Value.QuizInfos;
                if (infos.Count == 0)
                {
                    Error = "No quizzes found";
                }
                else
                {
                    Error = null;
                    Quizzes.AddRange(infos);
                }
            }
            else
            {
                Error = ownedQuizzesReply.StatusDetail;
            }
        }
    }
}
