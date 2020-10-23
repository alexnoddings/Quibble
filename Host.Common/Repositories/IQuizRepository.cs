using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Repositories
{
    public interface IQuizRepository : IEntityRepository<Guid, DbQuiz>
    {
        public Task<DateTime> PublishAsync(Guid id);
        public Task UpdateTitleAsync(Guid id, string newTitle);
        public Task<List<(string, Guid)>> GetQuizzesByUserAsync(Guid userId);
        public Task<List<(string, Guid)>> GetQuizzesParticipatedInAsync(Guid userId);
    }
}
