using System;
using System.Threading.Tasks;

namespace Quibble.Common.Quizzes
{
    public interface IQuizHubClient
    {
        public Task OnQuizUpdated(Quiz quiz);

        public Task OnQuizDeleted(Guid quizId);
    }
}
