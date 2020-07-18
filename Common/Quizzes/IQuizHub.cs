using System;
using System.Threading.Tasks;

namespace Quibble.Common.Quizzes
{
    public interface IQuizHub
    {
        public Task<Quiz> CreateAsync(Quiz quiz);

        public Task<Quiz> GetAsync(Guid id);

        public Task<Quiz> UpdateAsync(Quiz quiz);

        public Task DeleteAsync(Guid id);

        public Task RegisterForUpdatesAsync(Guid id);
    }
}
