using System;
using System.Threading.Tasks;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Repositories
{
    public interface IQuizRepository : IEntityRepository<Guid, DbQuiz>
    {
        public Task PublishAsync(Guid id);
        public Task UpdateTitleAsync(Guid id, string newTitle);
    }
}
