using System;
using System.Threading.Tasks;

namespace Quibble.Common.Rounds
{
    public interface IRoundHub
    {
        public Task<Round> CreateAsync(Round round);

        public Task<Round> GetAsync(Guid id);

        public Task<RoundFull> GetFullAsync(Guid id);

        public Task<Round> UpdateAsync(Round round);

        public Task DeleteAsync(Guid id);

        public Task RegisterForUpdatesAsync(Guid quizId);
    }
}
