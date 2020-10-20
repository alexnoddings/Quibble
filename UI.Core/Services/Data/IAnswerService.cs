using Quibble.Core.Entities;
using Quibble.UI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quibble.UI.Core.Services.Data
{
    public interface IAnswerService
    {
        public Task<List<DtoAnswer>> GetForQuizAsync(Guid quizId);

        public Task UpdateTextAsync(Guid id, string newText, Guid initiatorToken);
        public Task UpdateMarkAsync(Guid id, AnswerMark newMark);
    }
}
