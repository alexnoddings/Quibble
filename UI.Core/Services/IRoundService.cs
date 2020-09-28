using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Core.Services
{
    public interface IRoundService
    {
        public Task CreateAsync(Guid parentQuizId);

        public Task<List<DtoRound>> GetForQuizAsync(Guid quizId);

        public Task DeleteAsync(Guid id);

        public Task UpdateTitleAsync(Guid id, string newTitle);
    }
}
