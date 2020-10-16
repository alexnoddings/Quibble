using Quibble.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Quibble.UI.Core.Services.Data
{
    public interface IAnswerService
    {
        public Task UpdateTextAsync(Guid id, string newText);
        public Task UpdateMarkAsync(Guid id, AnswerMark newMark);
    }
}
