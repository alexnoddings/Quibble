using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Quibble.Common.Protos;

namespace Quibble.Server.Grpc
{
    internal static class GrpcReplyHelper
    {
        private const string DefaultInvalidArgumentDetail = "One or more values were invalid";
        private const string DefaultNotFoundDetail = "Resource was not found";
        private const string DefaultPermissionDeniedDetail = "You are not allowed to perform this action";
        private const string DefaultUnimplementedDetail = "This action is not implemented yet";
        private const string DefaultUnauthenticatedDetail = "You must be logged in to perform this action";

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.InvalidArgument"/> and creates an empty <typeparamref name="TReply"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static TReply InvalidArgument<TReply>(ServerCallContext context, string detail = DefaultInvalidArgumentDetail)
            where TReply : class, IMessage =>
            CreateReply<TReply>(context, StatusCode.InvalidArgument, detail);

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.InvalidArgument"/> and creates an empty <see cref="EmptyMessage"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static EmptyMessage InvalidArgument(ServerCallContext context, string detail = DefaultInvalidArgumentDetail) =>
            InvalidArgument<EmptyMessage>(context, detail);

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.NotFound"/> and creates an empty <typeparamref name="TReply"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static TReply NotFound<TReply>(ServerCallContext context, string detail = DefaultNotFoundDetail)
            where TReply : class, IMessage =>
            CreateReply<TReply>(context, StatusCode.NotFound, detail);

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.NotFound"/> and creates an empty <see cref="EmptyMessage"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static EmptyMessage NotFound(ServerCallContext context, string detail = DefaultNotFoundDetail) =>
            NotFound<EmptyMessage>(context, detail);

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.PermissionDenied"/> and creates an empty <typeparamref name="TReply"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static TReply PermissionDenied<TReply>(ServerCallContext context, string detail = DefaultPermissionDeniedDetail)
            where TReply : class, IMessage =>
            CreateReply<TReply>(context, StatusCode.PermissionDenied, detail);

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.PermissionDenied"/> and creates an empty <see cref="EmptyMessage"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static EmptyMessage PermissionDenied(ServerCallContext context, string detail = DefaultPermissionDeniedDetail) =>
            PermissionDenied<EmptyMessage>(context, detail);

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.Unimplemented"/> and creates an empty <typeparamref name="TReply"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static TReply Unimplemented<TReply>(ServerCallContext context, string detail = DefaultUnimplementedDetail)
            where TReply : class, IMessage =>
            CreateReply<TReply>(context, StatusCode.Unimplemented, detail);

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.Unimplemented"/> and creates an empty <see cref="EmptyMessage"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static EmptyMessage Unimplemented(ServerCallContext context, string detail = DefaultUnimplementedDetail) =>
            Unimplemented<EmptyMessage>(context, detail);

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.Unimplemented"/> and creates an empty <typeparamref name="TReply"/>.
        /// </summary>
        /// <returns>A completed task. This removes the need for a caller to use <code>Task.FromResult</code>.</returns>
        /// <inheritdoc cref="Unimplemented{TReply}(ServerCallContext, string)"/>
        internal static Task<TReply> UnimplementedAsync<TReply>(ServerCallContext context, string detail = DefaultUnimplementedDetail)
            where TReply : class, IMessage =>
            Task.FromResult(CreateReply<TReply>(context, StatusCode.Unimplemented, detail));

        /// <summary>
        /// Sets the context status as <see cref="StatusCode.Unimplemented"/> and creates an empty <see cref="EmptyMessage"/>.
        /// </summary>
        /// <returns>A completed task. This removes the need for a caller to use <code>Task.FromResult</code>.</returns>
        /// <inheritdoc cref="Unimplemented{TReply}(ServerCallContext, string)"/>
        internal static Task<EmptyMessage> UnimplementedAsync(ServerCallContext context, string detail = DefaultUnimplementedDetail) =>
            Task.FromResult(Unimplemented(context, detail));

        /// <summary>
        /// Sets the context status as <code>Unauthenticated</code> and creates an empty <typeparamref name="TReply"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static TReply Unauthenticated<TReply>(ServerCallContext context, string detail = DefaultUnauthenticatedDetail)
            where TReply : class, IMessage =>
            CreateReply<TReply>(context, StatusCode.Unauthenticated, detail);

        /// <summary>
        /// Sets the context status as <code>Unauthenticated</code> and creates an empty <see cref="EmptyMessage"/>.
        /// </summary>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, StatusCode, string)"/>
        internal static EmptyMessage Unauthenticated(ServerCallContext context, string detail = DefaultUnauthenticatedDetail) =>
            Unauthenticated<EmptyMessage>(context, detail);

        /// <param name="context">The context to modify.</param>
        /// <param name="statusCode">The status code of the operation.</param>
        /// <param name="detail">A message describing the operations result.</param>
        /// <inheritdoc cref="CreateReply{TReply}(ServerCallContext, Status)"/>
        internal static TReply CreateReply<TReply>(ServerCallContext context, StatusCode statusCode, string detail)
            where TReply : class, IMessage =>
            CreateReply<TReply>(context, new Status(statusCode, detail));

        /// <summary>
        /// Sets the context status and creates an empty reply.
        /// </summary>
        /// <typeparam name="TReply">The type of reply message.</typeparam>
        /// <param name="context">The context to modify.</param>
        /// <param name="status">The status to set on the context.</param>
        /// <returns>An empty reply message of type <typeparamref name="TReply"/>.</returns>
        internal static TReply CreateReply<TReply>(ServerCallContext context, Status status)
            where TReply : class, IMessage
        {
            context.Status = status;
            return Activator.CreateInstance<TReply>();
        }

        internal static EmptyMessage EmptyMessage { get; } = new EmptyMessage();
    }
}
