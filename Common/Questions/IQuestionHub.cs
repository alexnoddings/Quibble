using System;
using System.Threading.Tasks;

namespace Quibble.Common.Questions
{
    public interface IQuestionHub
    {
        public Task<Question> CreateAsync(Question question);

        public Task<Question> GetAsync(Guid id);

        public Task<Question> UpdateAsync(Question question);

        public Task DeleteAsync(Guid id);

        public Task RegisterForUpdatesAsync(Guid quizId);
    }
}
