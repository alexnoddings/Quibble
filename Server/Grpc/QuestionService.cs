using Quibble.Common.Protos;
using Quibble.Server.Extensions.Models;
using Quibble.Server.Models.Questions;

namespace Quibble.Server.Grpc
{
    public class QuestionService : Common.Protos.QuestionService.QuestionServiceBase
    {
        /// <summary>
        /// Converts a <see cref="Question"/> to a <see cref="QuestionInfo"/>.
        /// </summary>
        /// <param name="question">The <see cref="Question"/> to convert.</param>
        /// <returns>The converted <see cref="QuestionInfo"/>.</returns>
        internal static QuestionInfo ToQuestionInfo(Question question) =>
            new QuestionInfo
            {
                Id = question.Id.ToString(),
                RoundId = question.RoundId.ToString(),
                Body = question.Body,
                Answer = question.Answer,
                State = question.State.ToProtoEnum()
            };
    }
}
