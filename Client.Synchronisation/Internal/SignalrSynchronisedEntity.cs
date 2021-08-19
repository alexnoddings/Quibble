using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.Internal
{
    public abstract class SignalrSynchronisedEntity : BaseSynchronisedEntity, IDisposable
    {
        private HubConnection? _hubConnection;
        protected HubConnection HubConnection
        {
            get
            {
                if (IsDisposed || _hubConnection is null)
                    throw new ObjectDisposedException(GetType().Name);
                return _hubConnection;
            }
        }

        protected List<IDisposable> EventHandlers { get; } = new();

        protected bool IsDisposed { get; private set; }

        protected SignalrSynchronisedEntity(ILogger<BaseSynchronisedEntity> logger, HubConnection hubConnection)
               : base(logger)
        {
            _hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
        }

        private void AddEventHandler(IDisposable handler) =>
            EventHandlers.Add(handler);

        protected void AddEventHandler(Expression<Func<IQuibbleHubClient, Func<Task>>> eventSelector, Func<Task> eventHandler)
        {
            string eventName = GetMethodName(eventSelector);
            Task WrappedEventHandler() => Wrap(eventName, eventHandler);
            AddEventHandler(HubConnection.On(GetMethodName(eventSelector), WrappedEventHandler));
        }

        protected void AddEventHandler<T1>(Expression<Func<IQuibbleHubClient, Func<T1, Task>>> eventSelector, Func<T1, Task> eventHandler)
        {
            string eventName = GetMethodName(eventSelector);
            Task WrappedEventHandler(T1 t1) => Wrap(eventName, () => eventHandler(t1));
            AddEventHandler(HubConnection.On(eventName, (Func<T1, Task>)WrappedEventHandler));
        }

        protected void AddEventHandler<T1, T2>(Expression<Func<IQuibbleHubClient, Func<T1, T2, Task>>> eventSelector, Func<T1, T2, Task> eventHandler)
        {
            string eventName = GetMethodName(eventSelector);
            Task WrappedEventHandler(T1 t1, T2 t2) => Wrap(eventName, () => eventHandler(t1, t2));
            AddEventHandler(HubConnection.On<T1, T2>(eventName, WrappedEventHandler));
        }

        protected void AddFilteredEventHandler(Expression<Func<IQuibbleHubClient, Func<Guid, Task>>> eventSelector, Func<Task> eventHandler)
        {
            string eventName = GetMethodName(eventSelector);
            Task WrappedEventHandler(Guid id) => Wrap(eventName, () => id == Id ? eventHandler() : Task.CompletedTask);
            AddEventHandler(HubConnection.On<Guid>(eventName, WrappedEventHandler));
        }

        protected void AddFilteredEventHandler<T1>(Expression<Func<IQuibbleHubClient, Func<Guid, T1, Task>>> eventSelector, Func<T1, Task> eventHandler)
        {
            string eventName = GetMethodName(eventSelector);
            Task WrappedEventHandler(Guid id, T1 t1) => Wrap(eventName, () => id == Id ? eventHandler(t1) : Task.CompletedTask);
            AddEventHandler(HubConnection.On<Guid, T1>(eventName, WrappedEventHandler));
        }

        protected void AddFilteredEventHandler<T1, T2>(Expression<Func<IQuibbleHubClient, Func<Guid, T1, Task>>> eventSelector, Func<T1, T2, Task> eventHandler)
        {
            string eventName = GetMethodName(eventSelector);
            Task WrappedEventHandler(Guid id, T1 t1, T2 t2) => Wrap(eventName, () => id == Id ? eventHandler(t1, t2) : Task.CompletedTask);
            AddEventHandler(HubConnection.On<Guid, T1, T2>(eventName, WrappedEventHandler));
        }

        private async Task Wrap(string eventName, Func<Task> eventTask)
        {
            try
            {
                await eventTask();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Exception occurred while handling event {eventName} in {GetType().Name}");
                throw;
            }
        }

        private static string GetMethodName(LambdaExpression eventMethodSelector)
        {
            var methodInfo = GetMethodInfo(eventMethodSelector);
            if (methodInfo is null)
                throw new ArgumentException("Unsupported event selector.", nameof(eventMethodSelector));
            return methodInfo.Name;
        }

        private static MethodInfo? GetMethodInfo(LambdaExpression eventMethodSelector)
        {
            if (eventMethodSelector.Body is not UnaryExpression unaryExpression) return null;
            if (unaryExpression.Operand is not MethodCallExpression methodCallExpression) return null;
            if (methodCallExpression.Object is not ConstantExpression constantExpression) return null;
            if (constantExpression.Value is not MethodInfo methodInfo) return null;

            return methodInfo;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                while (EventHandlers.Count > 0)
                {
                    var handler = EventHandlers[0];
                    handler.Dispose();
                    EventHandlers.RemoveAt(0);
                }

                _hubConnection = null;
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
