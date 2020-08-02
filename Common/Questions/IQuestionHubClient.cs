using System;
using System.Threading.Tasks;

namespace Quibble.Common.Questions
{
    public interface IQuestionHubClient
    {
        public Task OnQuestionCreated(Question question);
        public Task OnQuestionUpdated(Question question);
        public Task OnQuestionDeleted(Guid questionId);
    }
}
