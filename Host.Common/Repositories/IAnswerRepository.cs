using Quibble.Core.Entities;
using Quibble.Host.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quibble.Host.Common.Repositories
{
    public interface IAnswerRepository
    {
        public Task<DbParticipantAnswer> GetAsync(Guid id);
        public Task<List<DbParticipantAnswer>> GetForQuizAsync(Guid quizId);
        public Task<List<DbParticipantAnswer>> GetForQuizAndUserAsync(Guid quizId, Guid userId);
        public Task UpdateAnswerTextAsync(Guid id, string newText);
        public Task UpdateAnswerMarkAsync(Guid id, AnswerMark newMark);
    }
}
