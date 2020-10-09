using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Core.Services.Data
{
    public interface IRoundService
    {
        public Task CreateAsync(Guid parentQuizId);

        public Task<List<DtoRound>> GetForQuizAsync(Guid quizId);

        public Task DeleteAsync(Guid id);

        public Task UpdateTitleAsync(Guid id, string newTitle);
        public Task UpdateStateAsync(Guid id, RoundState newState);
    }
}
