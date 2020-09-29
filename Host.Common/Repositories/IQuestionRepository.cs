using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Repositories
{
    public interface IQuestionRepository : IEntityRepository<Guid, DbQuestion>
    {
        public Task<List<DbQuestion>> GetForQuizAsync(Guid quizId);
        public Task UpdateTextAsync(Guid id, string newText);
        public Task UpdateAnswerAsync(Guid id, string newAnswer);
    }
}
