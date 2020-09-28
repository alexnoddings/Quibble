using System;
using System.Threading.Tasks;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Core.Services
{
    public interface ISynchronisedQuizFactory
    {
        Task<SyncedQuiz> GetAsync(Guid quizId);
    }
}