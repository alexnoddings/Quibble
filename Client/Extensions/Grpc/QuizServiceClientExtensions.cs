using System;
using System.Threading.Tasks;
using Google.Protobuf;
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
        private static readonly EmptyMessage EmptyMessage = new EmptyMessage();

        /// <summary>
        /// Creates a quiz.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <param name="title">The title of the quiz.</param>
        /// <returns>A <see cref="GrpcReply{QuizInfo}"/> representing the result of the call.</returns>
        /// <seealso cref="QuizService.QuizServiceClient.CreateAsync(CreateQuizRequest,CallOptions)"/>
        public static Task<GrpcReply<QuizInfo>> CreateAsync(this QuizService.QuizServiceClient client, string title)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (title == null) throw new ArgumentNullException(nameof(title));

            var request = new CreateQuizRequest { Title = title };
            return RunAsync(async () => await client.CreateAsync(request));
        }

        /// <summary>
        /// Gets a <see cref="QuizInfo"/>.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <param name="id">The identifier for the quiz.</param>
        /// <returns></returns>
        public static Task<GrpcReply<QuizInfo>> GetAsync(this QuizService.QuizServiceClient client, string id)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new GetQuizRequest { Id = id };
            return RunAsync(async () => await client.GetAsync(request));
        }

        /// <summary>
        /// Gets the <see cref="QuizInfo"/>s for quizzes owned by the calling user.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <returns>The <see cref="QuizInfo"/>s for quizzes owned by the calling user.</returns>
        public static Task<GrpcReply<GetOwnedQuizzesReply>> GetOwnedAsync(this QuizService.QuizServiceClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            return RunAsync(async () => await client.GetOwnedAsync(EmptyMessage));
        }

        /// <summary>
        /// Updates the title for a quiz.
        /// </summary>
        /// <param name="client">The <see cref="QuizService.QuizServiceClient"/>.</param>
        /// <param name="id">The identifier for the quiz.</param>
        /// <param name="newTitle">The new title for the quiz.</param>
        /// <returns></returns>
        public static Task<GrpcReply> UpdateTitleAsync(this QuizService.QuizServiceClient client, string id, string newTitle)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (newTitle == null) throw new ArgumentNullException(nameof(newTitle));

            var request = new UpdateQuizTitleRequest {Id = id, NewTitle = newTitle};
            return RunAsync(async () => await client.UpdateTitleAsync(request));
        }

        /// <summary>
        /// Runs a function which returns an empty message.
        /// </summary>
        /// <param name="exec">A function which returns a <see cref="Task{EmptyMessage}"/> when executed.</param>
        /// <returns>A <see cref="GrpcReply"/> indicating the result of the call.</returns>
        /// <seealso cref="RunAsync{TReply}"/>
        public static async Task<GrpcReply> RunAsync(Func<Task<EmptyMessage>> exec) => 
            await RunAsync<EmptyMessage>(exec).ConfigureAwait(false);

        /// <summary>
        /// Runs a function which returns a response of type <typeparamref name="TReply"/>.
        /// </summary>
        /// <typeparam name="TReply">The type of the reply message.</typeparam>
        /// <param name="exec">A function which returns a <see cref="Task{TReply}"/> when executed.</param>
        /// <returns>A <see cref="GrpcReply{TReply}"/> indicating the result of the call.</returns>
        /// <remarks>
        ///     <para>
        ///         This wraps the <paramref name="exec"/> to catch <see cref="RpcException"/>s thrown during execution.
        ///     </para>
        ///     <para>
        ///         Special cases are considered for when the user is required to be logged in and if a network connection could not be formed.
        ///     </para>
        ///     <para>
        ///         <see cref="GrpcReply{TReply}.Value"/> is never <c>null</c>.
        ///         It will be initialised as an empty/default reply when <see cref="GrpcReply.Ok"/> is <c>false</c>.
        ///     </para>
        /// </remarks>
        public static async Task<GrpcReply<TReply>> RunAsync<TReply>(Func<Task<TReply>> exec)
            where TReply : class, IMessage<TReply>
        {
            if (exec == null) throw new ArgumentNullException(nameof(exec));

            TReply? reply = null;
            var statusCode = StatusCode.OK;
            string statusDetail = "OK";

            try
            {
                reply = await exec().ConfigureAwait(false);
            }
            // e.InnerException is not set correctly, so specific cases need to check the status code and detail
            catch (RpcException e) when (e.StatusCode == StatusCode.Internal && e.Status.Detail.Contains("AccessTokenNotAvailableException", StringComparison.OrdinalIgnoreCase))
            {
                // AccessTokenNotAvailableException is thrown when the user is not logged in
                statusCode = StatusCode.Unauthenticated;
                statusDetail = "You must be logged in to access this";
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Internal && e.Status.Detail.Contains("NetworkError when attempting to fetch resource", StringComparison.OrdinalIgnoreCase))
            {
                // JSException for NetworkError is thrown when unable to connect to the server
                statusCode = StatusCode.Aborted;
                statusDetail = "Could not connect to the server";
            }
            catch (RpcException e)
            {
                statusCode = e.Status.StatusCode;
                statusDetail = e.Status.Detail;
            }

            // Grpc messages all support empty constructors
            reply ??= Activator.CreateInstance<TReply>();

            return new GrpcReply<TReply>(statusCode, statusDetail, reply);
        }
    }
}