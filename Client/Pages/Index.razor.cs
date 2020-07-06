using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Client.Extensions.Grpc;

namespace Quibble.Client.Pages
{
    public partial class Index
    {
        private List<(Guid, string)> OwnedQuizzes { get; } = new List<(Guid, string)>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var quizzesReply = await QuizClient.GetOwnedAsync().ConfigureAwait(false);
            if (quizzesReply.OK)
            {
                foreach (var quiz in quizzesReply.Value.QuizInfos)
                {
                    Guid id = Guid.Parse(quiz.Id);
                    OwnedQuizzes.Add((id, quiz.Title));
                }
            }
            else
            {
                Console.WriteLine($"err {quizzesReply.StatusCode}: {quizzesReply.StatusDetail}");
            }
        }
    }
}
