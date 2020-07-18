using System;
using System.Threading.Tasks;

namespace Quibble.Common.Questions
{
    public interface IQuestionHub
    {
        public Task<Question> CreateAsync(Question quiz);

        public Task<Question> GetAsync(Guid id);

        public Task<Question> UpdateAsync(Question quiz);

        public Task DeleteAsync(Guid id);
    }
}
