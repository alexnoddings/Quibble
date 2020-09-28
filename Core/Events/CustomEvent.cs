using System;
using System.Collections.Immutable;

namespace Quibble.Core.Events
{
    public class CustomEvent<T> where T : class
    {
        protected object SubscriptionLock { get; } = new object();
        protected ImmutableArray<T> Subscriptions { get; private set; } = ImmutableArray.Create<T>();

        public void Add(T subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));
            lock (SubscriptionLock)
                Subscriptions = Subscriptions.Add(subscriber);
        }

        public void Remove(T subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));
            lock (SubscriptionLock)
                Subscriptions = Subscriptions.Remove(subscriber);
        }
    }
}
