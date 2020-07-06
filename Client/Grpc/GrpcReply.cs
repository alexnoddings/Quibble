using System;
using Google.Protobuf;
using Grpc.Core;
using Quibble.Common.Protos;

namespace Quibble.Client.Grpc
{
    /// <summary>
    /// Represents a reply from a gRPC call.
    /// </summary>
    /// <remarks>
    /// For calls which return something other than <see cref="EmptyMessage"/>, the <see cref="GrpcReply{TReply}"/> class should be used.
    /// </remarks>
    public class GrpcReply
    {
        /// <summary>
        /// The <see cref="StatusCode"/> representing the result of the call.
        /// </summary>
        public StatusCode StatusCode { get; }

        /// <summary>
        /// Gets whether the call was successful.
        /// </summary>
        public bool Ok => StatusCode == StatusCode.OK;

        /// <summary>
        /// Gets a detail message representing the result of the call. These may be displayed to the user.
        /// </summary>
        public string StatusDetail { get; }

        /// <summary>
        /// Initialises a new <see cref="GrpcReply"/> with the specified <see cref="StatusCode"/> and status detail message.
        /// </summary>
        /// <param name="statusCode">The <see cref="StatusCode"/> representing the result of the call.</param>
        /// <param name="statusDetail">The detail message representing the result of the call.</param>
        public GrpcReply(StatusCode statusCode, string statusDetail)
        {
            StatusCode = statusCode;
            StatusDetail = statusDetail ?? throw new ArgumentNullException(nameof(statusDetail));
        }
    }

    /// <summary>
    /// Represents a reply from a gRPC call which returned a value.
    /// </summary>
    /// <remarks>
    /// For calls which return <see cref="EmptyMessage"/>, the <see cref="GrpcReply"/> class should be used.
    /// </remarks>
    /// <typeparam name="TReply">The type of value returned by the call.</typeparam>
    public class GrpcReply<TReply> : GrpcReply
        where TReply : class, IMessage<TReply>
    {
        /// <summary>
        /// Gets the value returned from the call.
        /// </summary>
        /// <remarks>
        ///     <see cref="Value"/> is never <c>null</c>.
        ///     It will be initialised as an empty/default reply when <see cref="GrpcReply.Ok"/> is <c>false</c>.
        /// </remarks>
        public TReply Value { get; }

        /// <summary>
        /// Initialises a new <see cref="GrpcReply{TReply}"/> with the specified <see cref="StatusCode"/>, status detail message, and value.
        /// </summary>
        /// <param name="statusCode">The <see cref="StatusCode"/> representing the result of the call.</param>
        /// <param name="statusDetail">The detail message representing the result of the call.</param>
        /// <param name="value">The value returned by the call. This cannot be null.</param>
        public GrpcReply(StatusCode statusCode, string statusDetail, TReply value) 
            : base(statusCode, statusDetail)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
