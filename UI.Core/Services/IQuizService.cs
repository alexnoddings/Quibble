using System;
using System.Threading.Tasks;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Core.Services

{
    public interface IQuizService
    {
        public Task<Guid> CreateAsync(string title);

        public Task<DtoQuiz> GetAsync(Guid id);

        public Task PublishAsync(Guid id);

        public Task DeleteAsync(Guid id);

        public Task UpdateTitleAsync(Guid id, string newTitle);
    }
}
