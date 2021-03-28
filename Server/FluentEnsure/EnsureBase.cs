using System;
using System.Diagnostics;

namespace Quibble.Server.FluentEnsure
{
    public abstract class EnsureBase<TEntity>
    {
        protected readonly TEntity Entity;
        protected readonly Guid UserId;

        protected EnsureBase(TEntity entity, Guid userId)
        {
            Entity = entity;
            UserId = userId;
        }

        [DebuggerStepThrough]
        protected EnsureChained<TEntity> EnsureSuccess(EnsureDelegate<TEntity> ensure, string error)
        {
            bool success = ensure.Invoke(Entity, UserId);
            if (!success)
                throw new InvalidOperationException(error);
            return new EnsureChained<TEntity>(Entity, UserId);
        }
    }
}
