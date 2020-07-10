using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Quibble.Client.Grpc;
using Quibble.Common.Protos;

namespace Quibble.Client.Extensions.Grpc
{
    /// <summary>
    /// Provides methods for gRPC client extensions.
    /// </summary>
    internal static class GrpcClientExtensionHelpers
    {
        /// <summary>
        /// An empty message for gRPC calls which don't require data for a request or reply.
        /// </summary>
        public static readonly EmptyMessage EmptyMessage = new EmptyMessage();

        /// <summary>
        /// Executes a function which takes a <see cref="EmptyMessage"/> and returns a <see cref="AsyncUnaryCall{EmptyMessage}"/>.
        /// </summary>
        /// <param name="exec">A function which returns a <see cref="Task{EmptyMessage}"/> when executed.</param>
        /// <param name="headers">The request <seealso cref="Metadata"/>.</param>
        /// <param name="deadline">The request <seealso cref="DateTime"/> deadline.</param>
        /// <param name="cancellationToken">The request <seealso cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result <see cref="GrpcReply"/> represents the executed operation's result.</returns>
        /// <seealso cref="RunAsync{TRequest, TReply}"/>
        public static async Task<GrpcReply> RunAsync(Func<EmptyMessage, Metadata?, DateTime?, CancellationToken, AsyncUnaryCall<EmptyMessage>> exec, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            => await RunAsync<EmptyMessage, EmptyMessage>(exec, EmptyMessage, headers, deadline, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Executes a function which takes a <see cref="EmptyMessage"/> and returns a <see cref="AsyncUnaryCall{TReply}"/>.
        /// </summary>
        /// <typeparam name="TReply">The type of the reply message.</typeparam>
        /// <param name="exec">A function which returns a <see cref="Task{EmptyMessage}"/> when executed.</param>
        /// <param name="headers">The request <seealso cref="Metadata"/>.</param>
        /// <param name="deadline">The request <seealso cref="DateTime"/> deadline.</param>
        /// <param name="cancellationToken">The request <seealso cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result <see cref="GrpcReply{TReply}"/> represents the executed operation's result.</returns>
        /// <seealso cref="RunAsync{TRequest, TReply}"/>
        public static async Task<GrpcReply<TReply>> RunAsync<TReply>(Func<EmptyMessage, Metadata?, DateTime?, CancellationToken, AsyncUnaryCall<TReply>> exec, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            where TReply : class, IMessage<TReply>
            => await RunAsync<EmptyMessage, TReply>(exec, EmptyMessage, headers, deadline, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Executes a function which takes a <typeparamref name="TRequest"/> and returns a <see cref="AsyncUnaryCall{EmptyMessage}"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request message.</typeparam>
        /// <param name="exec">A function which returns a <see cref="Task{EmptyMessage}"/> when executed.</param>
        /// <param name="request">The request message to send in the <paramref name="exec"/>.</param>
        /// <param name="headers">The request <seealso cref="Metadata"/>.</param>
        /// <param name="deadline">The request <seealso cref="DateTime"/> deadline.</param>
        /// <param name="cancellationToken">The request <seealso cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result <see cref="GrpcReply"/> represents the executed operation's result.</returns>
        /// <seealso cref="RunAsync{TRequest, TReply}"/>
        public static async Task<GrpcReply> RunAsync<TRequest>(Func<TRequest, Metadata?, DateTime?, CancellationToken, AsyncUnaryCall<EmptyMessage>> exec, TRequest request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            where TRequest : class, IMessage<TRequest>
            => await RunAsync<TRequest, EmptyMessage>(exec, request, headers, deadline, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Executes a function which takes a <typeparamref name="TRequest"/> and returns a <see cref="GrpcReply{TReply}"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request message.</typeparam>
        /// <typeparam name="TReply">The type of the reply message.</typeparam>
        /// <param name="exec">A function which returns a <see cref="AsyncUnaryCall{TReply}"/> when executed. This should be a gRPC call.</param>
        /// <param name="request">The request message to send in the <paramref name="exec"/>.</param>
        /// <param name="headers">The request <seealso cref="Metadata"/>.</param>
        /// <param name="deadline">The request <seealso cref="DateTime"/> deadline.</param>
        /// <param name="cancellationToken">The request <seealso cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result <see cref="GrpcReply{TReply}"/> represents the executed operation's result.</returns>
        /// <remarks>
        ///     <para>This wraps the <paramref name="exec"/> to catch <see cref="RpcException"/>s thrown during execution.</para>
        ///     <para>Special cases are considered for when the user is required to be logged in and if a network connection could not be formed.</para>
        ///     <para><see cref="GrpcReply{TReply}.Value"/> is never <c>null</c>. It will be initialised as an empty/default reply when <see cref="GrpcReply.Ok"/> is <c>false</c>.</para>
        /// </remarks>
        public static async Task<GrpcReply<TReply>> RunAsync<TRequest, TReply>(Func<TRequest, Metadata?, DateTime?, CancellationToken, AsyncUnaryCall<TReply>> exec, TRequest request, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
            where TRequest : class, IMessage<TRequest>
            where TReply : class, IMessage<TReply>
        {
            if (exec == null) throw new ArgumentNullException(nameof(exec));
            if (request == null) throw new ArgumentNullException(nameof(request));

            TReply? reply = null;
            var statusCode = StatusCode.OK;
            string statusDetail = "OK";

            try
            {
                reply = await exec(request, headers, deadline, cancellationToken);
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
            catch (RpcException e) when (e.StatusCode == StatusCode.Unknown && e.Status.Detail.Contains("Exception was thrown by handler", StringComparison.OrdinalIgnoreCase))
            {
                // Exception was thrown by handler is the detail returned when an unhandled exception is thrown on the server
                statusCode = StatusCode.Unknown;
                statusDetail = "An unknown error occured on the server";
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
