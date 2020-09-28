using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quibble.Core.Events
{
    public class AsyncEvent : CustomEvent<Func<Task>>
    {
        public Task InvokeAsync()
        {
            IEnumerable<Task> tasks;
            lock (SubscriptionLock)
                tasks = Subscriptions.Select(s => s.Invoke());
            return Task.WhenAll(tasks);
        }
    }

    public class AsyncEvent<T1> : CustomEvent<Func<T1, Task>>
    {
        public Task InvokeAsync(T1 arg)
        {
            IEnumerable<Task> tasks;
            lock (SubscriptionLock)
                tasks = Subscriptions.Select(s => s.Invoke(arg));
            return Task.WhenAll(tasks);
        }
    }

    public class AsyncEvent<T1, T2> : CustomEvent<Func<T1, T2, Task>>
    {
        public Task InvokeAsync(T1 arg1, T2 arg2)
        {
            IEnumerable<Task> tasks;
            lock (SubscriptionLock)
                tasks = Subscriptions.Select(s => s.Invoke(arg1, arg2));
            return Task.WhenAll(tasks);
        }
    }

    public class AsyncEvent<T1, T2, T3> : CustomEvent<Func<T1, T2, T3, Task>>
    {
        public Task InvokeAsync(T1 arg1, T2 arg2, T3 arg3)
        {
            IEnumerable<Task> tasks;
            lock (SubscriptionLock)
                tasks = Subscriptions.Select(s => s.Invoke(arg1, arg2, arg3));
            return Task.WhenAll(tasks);
        }
    }
}
