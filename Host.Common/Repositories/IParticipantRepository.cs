using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Repositories
{
    public interface IParticipantRepository : IEntityRepository<Guid, DbParticipant>
    {
        public Task<bool> IsParticipatingAsync(Guid quizId, Guid userId);
        public Task<DbParticipant> GetAsync(Guid quizId, Guid userId);
        public Task<List<DbParticipant>> GetForQuizAsync(Guid quizId);
    }
}
