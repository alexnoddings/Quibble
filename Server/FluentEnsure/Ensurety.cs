namespace Quibble.Server.FluentEnsure
{
    public readonly struct Ensurety<TEntity>
    {
        public EnsureDelegate<TEntity> Ensure { get; }
        public string Error { get; }

        public Ensurety(EnsureDelegate<TEntity> ensure, string error)
        {
            Ensure = ensure;
            Error = error;
        }
    }
}
