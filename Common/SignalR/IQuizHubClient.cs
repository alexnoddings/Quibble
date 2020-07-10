using System.Threading.Tasks;
using Quibble.Common.Protos;

namespace Quibble.Common.SignalR
{
    /// <summary>
    /// Provides the actions available to perform on clients of the Quiz SignalR hub.
    /// </summary>
    public interface IQuizHubClient
    {
        /// <summary>
        /// Called when a Quiz is updated.
        /// </summary>
        /// <param name="quizInfo">The updated <see cref="QuizInfo"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task OnQuizUpdated(QuizInfo quizInfo);

        /// <summary>
        /// Called when a Quiz is deleted.
        /// </summary>
        /// <param name="quizId">The identifier of the Quiz.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task OnQuizDeleted(string quizId);

        /// <summary>
        /// Called when a new Round is created.
        /// </summary>
        /// <param name="roundInfo">The updated <see cref="RoundInfo"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task OnRoundCreated(RoundInfo roundInfo);

        /// <summary>
        /// Called when a Round is updated.
        /// </summary>
        /// <param name="roundInfo">The updated <see cref="RoundInfo"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task OnRoundUpdated(RoundInfo roundInfo);

        /// <summary>
        /// Called when a Round is deleted.
        /// </summary>
        /// <param name="roundId">The identifier of the Round.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task OnRoundDeleted(string roundId);

        /// <summary>
        /// Called when a new Question is created.
        /// </summary>
        /// <param name="questionInfo">The updated <see cref="QuestionInfo"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task OnQuestionCreated(QuestionInfo questionInfo);

        /// <summary>
        /// Called when a Question is updated.
        /// </summary>
        /// <param name="questionInfo">The updated <see cref="QuestionInfo"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task OnQuestionUpdated(QuestionInfo questionInfo);

        /// <summary>
        /// Called when a Question is deleted.
        /// </summary>
        /// <param name="roundId">The identifier of the Question's parent Round.</param>
        /// <param name="questionId">The identifier of the Question.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task OnQuestionDeleted(string roundId, string questionId);
    }
}
