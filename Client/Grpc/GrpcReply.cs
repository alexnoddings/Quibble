using System;
using Grpc.Core;

namespace Quibble.Client.Grpc
{
    public class GrpcReply
    {
        public StatusCode StatusCode { get; }

        public bool OK => StatusCode == StatusCode.OK;

        public string StatusDetail { get; }

        public GrpcReply(StatusCode statusCode, string statusDetail)
        {
            StatusCode = statusCode;
            StatusDetail = statusDetail ?? throw new ArgumentNullException(nameof(statusDetail));
        }
    }

    public class GrpcReply<TReply> : GrpcReply
        where TReply : class
    {
        public TReply Value { get; }

        public GrpcReply(StatusCode statusCode, string statusDetail, TReply value) 
            : base(statusCode, statusDetail)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
