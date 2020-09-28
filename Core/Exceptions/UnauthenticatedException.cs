using System;
using System.Runtime.Serialization;

namespace Quibble.Core.Exceptions
{
    [Serializable]
    public class UnauthenticatedException : Exception
    {
        private const string DefaultMessage = "You must be authenticated to perform this operation.";

        protected UnauthenticatedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public UnauthenticatedException(string? message = null, Exception? innerException = null)
            : base(message ?? DefaultMessage, innerException)
        {
        }
    }
}
