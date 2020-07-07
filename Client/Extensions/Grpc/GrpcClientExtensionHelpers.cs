﻿using System;
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
        /// Runs a function which returns an empty message.
        /// </summary>
        /// <param name="exec">A function which returns a <see cref="Task{EmptyMessage}"/> when executed.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result <see cref="GrpcReply"/> represents the executed operation's result.</returns>
        /// <seealso cref="RunAsync{TReply}"/>
        public static async Task<GrpcReply> RunAsync(Func<Task<EmptyMessage>> exec) =>
            await RunAsync<EmptyMessage>(exec).ConfigureAwait(false);

        /// <summary>
        /// Runs a function which returns a response of type <typeparamref name="TReply"/>.
        /// </summary>
        /// <typeparam name="TReply">The type of the reply message.</typeparam>
        /// <param name="exec">A function which returns a <see cref="Task{TReply}"/> when executed.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result <see cref="GrpcReply{TReply}"/> represents the executed operation's result.</returns>
        /// <remarks>
        ///     <para>This wraps the <paramref name="exec"/> to catch <see cref="RpcException"/>s thrown during execution.</para>
        ///     <para>Special cases are considered for when the user is required to be logged in and if a network connection could not be formed.</para>
        ///     <para><see cref="GrpcReply{TReply}.Value"/> is never <c>null</c>. It will be initialised as an empty/default reply when <see cref="GrpcReply.Ok"/> is <c>false</c>.</para>
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