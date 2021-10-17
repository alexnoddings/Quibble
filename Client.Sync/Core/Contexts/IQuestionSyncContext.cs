using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Contexts
{
    public interface IQuestionSyncContext
    {
        public Task AddQuestionAsync(Guid roundId);
        public Task UpdateTextAsync(Guid id, string newText);
        public Task UpdateAnswerAsync(Guid id, string newAnswer);
        public Task UpdatePointsAsync(Guid id, decimal newPoints);
        public Task UpdateStateAsync(Guid id, QuestionState newState);
        public Task DeleteAsync(Guid id);

        public event Func<QuestionDto, Task> OnAddedAsync;
        public event Func<QuestionDto, List<SubmittedAnswerDto>, Task> OnRevealedAsync;
        public event Func<Guid, string, Task> OnTextUpdatedAsync;
        public event Func<Guid, string, Task> OnAnswerUpdatedAsync;
        public event Func<Guid, decimal, Task> OnPointsUpdatedAsync;
        public event Func<Guid, QuestionState, Task> OnStateUpdatedAsync;
        public event Func<Guid, int, Task> OnOrderUpdatedAsync;
        public event Func<Guid, Task> OnDeletedAsync;
    }
}
