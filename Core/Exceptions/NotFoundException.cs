using System;
using System.Runtime.Serialization;
using System.Text;

namespace Quibble.Core.Exceptions
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public string EntityTypeName { get; }
        public string? EntityId { get; }

        public NotFoundException(string? typeName = null, string? entityId = null, string? message = null, Exception? innerException = null)
            : base(GenerateErrorMessage(typeName ?? "Entity", entityId, message), innerException)
        {
            EntityTypeName = typeName ?? "Entity";
            EntityId = entityId;
        }

        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            EntityTypeName = "Entity";
        }

        private static string GenerateErrorMessage(string entityTypeName, string? entityId = null, string? message = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(entityTypeName);

            if (entityId != null)
            {
                sb.Append(" with id \"");
                sb.Append(entityId);
                sb.Append('\"');
            }

            sb.Append(" was not found");

            if (message != null)
            {
                sb.Append(": ");
                sb.Append(message);
            }

            sb.Append('.');

            return sb.ToString();
        }
    }
}
