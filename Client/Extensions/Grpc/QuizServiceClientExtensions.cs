using System;
using System.Threading.Tasks;
using Grpc.Core;
using Quibble.Client.Grpc;
using Quibble.Common.Protos;

namespace Quibble.Client.Extensions.Grpc
{
    /// <summary>
    /// Extensions methods for executing remote calls from a <see cref="QuizService.QuizServiceClient"/>.
    /// </summary>
    public static class QuizServiceClientExtensions
    {
        /// <summary>
        /// Creates a quiz.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <param name="title">The title of the quiz.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous creation operation. The task result <see cref="GrpcReply"/> represents the created quiz's <see cref="QuizInfo"/>.</returns>
        /// <seealso cref="QuizService.QuizServiceClient.CreateAsync(CreateQuizRequest,CallOptions)"/>
        public static Task<GrpcReply<QuizInfo>> CreateAsync(this QuizService.QuizServiceClient client, string title)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (title == null) throw new ArgumentNullException(nameof(title));

            var request = new CreateQuizRequest { Title = title };
            return GrpcClientExtensionHelpers.RunAsync(client.CreateAsync, request);
        }

        /// <summary>
        /// Gets a <see cref="QuizInfo"/>.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <param name="id">The identifier for the quiz.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result <see cref="GrpcReply"/> represents the found quiz's <see cref="QuizInfo"/>.</returns>
        public static Task<GrpcReply<QuizInfo>> GetInfoAsync(this QuizService.QuizServiceClient client, string id)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new EntityRequest { Id = id };
            return GrpcClientExtensionHelpers.RunAsync(client.GetInfoAsync, request);
        }

        /// <summary>
        /// Gets the <see cref="QuizInfo"/>s for quizzes owned by the calling user.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result <see cref="GrpcReply"/> represents the found quizzes's <see cref="QuizInfo"/>.</returns>
        public static Task<GrpcReply<GetOwnedInfosReply>> GetOwnedInfosAsync(this QuizService.QuizServiceClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            return GrpcClientExtensionHelpers.RunAsync(client.GetOwnedInfosAsync);
        }

        /// <summary>
        /// Gets a <see cref="QuizFull"/>.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <param name="id">The identifier for the quiz.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result <see cref="GrpcReply"/> represents the found quiz's <see cref="QuizFull"/>.</returns>
        public static Task<GrpcReply<QuizFull>> GetFullAsync(this QuizService.QuizServiceClient client, string id)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new EntityRequest { Id = id };
            return GrpcClientExtensionHelpers.RunAsync(client.GetFullAsync, request);
        }

        /// <summary>
        /// Updates the title for a quiz.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <param name="id">The identifier for the quiz.</param>
        /// <param name="newTitle">The new title for the quiz.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous update operation. The task result <see cref="GrpcReply"/> represents the updated quiz's <see cref="QuizInfo"/>.</returns>
        public static Task<GrpcReply> UpdateTitleAsync(this QuizService.QuizServiceClient client, string id, string newTitle)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (newTitle == null) throw new ArgumentNullException(nameof(newTitle));

            var request = new UpdateQuizTitleRequest {Id = id, NewTitle = newTitle};
            return GrpcClientExtensionHelpers.RunAsync(client.UpdateTitleAsync, request);
        }

        /// <summary>
        /// Deletes a quiz.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <param name="id">The identifier for the quiz.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous delete operation. The task result <see cref="GrpcReply"/> represents the delete quiz's status.</returns>
        public static Task<GrpcReply> DeleteAsync(this QuizService.QuizServiceClient client, string id)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new EntityRequest { Id = id };
            return GrpcClientExtensionHelpers.RunAsync(client.DeleteAsync, request);
        }
    }
}