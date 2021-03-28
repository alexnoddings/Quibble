using System;

namespace Quibble.Server.FluentEnsure
{
    public delegate bool EnsureDelegate<in TEntity>(TEntity entity, Guid currentUserId);

    public delegate bool PartialEnsureDelegate<in TEntity>(TEntity entity);
}
