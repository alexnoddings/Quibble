using System;
using System.Diagnostics;

namespace Quibble.Server.FluentEnsure
{
    public class EnsureFirst<TEntity> : EnsureBase<TEntity>
    {
        public EnsureFirst(TEntity entity, Guid userId)
            : base(entity, userId)
        {
        }

        [DebuggerStepThrough]
        public EnsureChained<TEntity> That(EnsureDelegate<TEntity> ensure, string error) =>
            EnsureSuccess(ensure, error);

        [DebuggerStepThrough]
        public EnsureChained<TEntity> That(PartialEnsureDelegate<TEntity> ensure, string error) =>
            EnsureSuccess((entity, _) => ensure(entity), error);

        [DebuggerStepThrough]
        public EnsureChained<TEntity> That(Ensurety<TEntity> predicate) =>
            EnsureSuccess(predicate.Ensure, predicate.Error);
    }
}
