using System;
using System.Runtime.Serialization;

namespace Quibble.Core.Exceptions
{
    [Serializable]
    public class UnauthorisedException : Exception
    {
        private const string DefaultMessage = "You are not authorised to perform this operation.";

        protected UnauthorisedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public UnauthorisedException(string? message = null, Exception? innerException = null)
            : base(message ?? DefaultMessage, innerException)
        {
        }
    }
}
