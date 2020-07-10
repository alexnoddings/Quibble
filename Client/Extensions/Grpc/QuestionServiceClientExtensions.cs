using System;
using System.Threading.Tasks;
using Grpc.Core;
using Quibble.Client.Grpc;
using Quibble.Common.Protos;

namespace Quibble.Client.Extensions.Grpc
{
    /// <summary>
    /// Extensions methods for executing remote calls from a <see cref="QuestionService.QuestionServiceClient"/>.
    /// </summary>
    public static class QuestionServiceClientExtensions
    {
        /// <summary>
        /// Creates a round.
        /// </summary>
        /// <param name="client">The <see cref="QuestionService.QuestionServiceClient"/>.</param>
        /// <param name="roundId">The identifier for the question's parent round.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous creation operation. The task result <see cref="GrpcReply"/> represents the created question's <see cref="QuestionInfo"/>.</returns>
        /// <seealso cref="QuestionService.QuestionServiceClient.CreateAsync(CreateQuestionRequest,CallOptions)"/>
        public static Task<GrpcReply<QuestionInfo>> CreateAsync(this QuestionService.QuestionServiceClient client, string roundId)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (roundId == null) throw new ArgumentNullException(nameof(roundId));

            var request = new CreateQuestionRequest { RoundId = roundId };
            return GrpcClientExtensionHelpers.RunAsync(client.CreateAsync, request);
        }

        /// <summary>
        /// Gets a <see cref="QuestionInfo"/>.
        /// </summary>
        /// <param name="client">The <see cref="QuestionService.QuestionServiceClient"/>.</param>
        /// <param name="id">The identifier for the question.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result <see cref="GrpcReply"/> represents the found question's <see cref="QuestionInfo"/>.</returns>
        public static Task<GrpcReply<QuestionInfo>> GetInfoAsync(this QuestionService.QuestionServiceClient client, string id)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new EntityRequest { Id = id };
            return GrpcClientExtensionHelpers.RunAsync(client.GetInfoAsync, request);
        }

        /// <summary>
        /// Updates a question.
        /// </summary>
        /// <param name="client">The <see cref="QuestionService.QuestionServiceClient"/>.</param>
        /// <param name="id">The identifier for the question.</param>
        /// <param name="newBody">The new body for the question.</param>
        /// <param name="newAnswer">The new answer for the question.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous update operation. The task result <see cref="GrpcReply"/> represents the updated question's <see cref="QuestionInfo"/>.</returns>
        public static Task<GrpcReply> UpdateAsync(this QuestionService.QuestionServiceClient client, string id, string newBody, string newAnswer)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new UpdateQuestionRequest { Id = id, NewBody = newBody, NewAnswer = newAnswer };
            return GrpcClientExtensionHelpers.RunAsync(client.UpdateAsync, request);
        }

        /// <summary>
        /// Deletes a round.
        /// </summary>
        /// <param name="client">The <see cref="QuestionService.QuestionServiceClient"/>.</param>
        /// <param name="id">The identifier for the question.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous delete operation. The task result <see cref="GrpcReply"/> represents the delete question's status.</returns>
        public static Task<GrpcReply> DeleteAsync(this QuestionService.QuestionServiceClient client, string id)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new EntityRequest { Id = id };
            return GrpcClientExtensionHelpers.RunAsync(client.DeleteAsync, request);
        }
    }
}
