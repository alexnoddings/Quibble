using System;
using System.Diagnostics;

namespace Quibble.Server.FluentEnsure
{
    public class EnsureChained<TEntity> : EnsureBase<TEntity>
    {
        public EnsureChained(TEntity entity, Guid userId)
            : base(entity, userId)
        {
        }

        [DebuggerStepThrough]
        public EnsureChained<TEntity> AndThat(EnsureDelegate<TEntity> ensure, string error) => 
            EnsureSuccess(ensure, error);

        [DebuggerStepThrough]
        public EnsureChained<TEntity> AndThat(PartialEnsureDelegate<TEntity> ensure, string error) => 
            EnsureSuccess((entity, _) => ensure(entity), error);

        [DebuggerStepThrough]
        public EnsureChained<TEntity> AndThat(Ensurety<TEntity> ensurety) => 
            EnsureSuccess(ensurety.Ensure, ensurety.Error);
    }
}
