using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Sync.Entities;
using Quibble.Shared.Hub;

namespace Quibble.Client.Sync.Internal
{
    public abstract class SynchronisedEntity : ISynchronisedEntity, IDisposable
    {
        public abstract Guid Id { get; }

        public event Func<Task>? Updated;

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

        protected SynchronisedEntity(HubConnection hubConnection)
        {
            _hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
        }

        private void AddEventHandler(IDisposable handler) => 
	        EventHandlers.Add(handler);

        protected void AddEventHandler(Expression<Func<IQuibbleHubClient, Func<Task>>> eventSelector, Func<Task> eventHandler) => 
            AddEventHandler(HubConnection.On(GetMethodName(eventSelector), eventHandler));

        protected void AddEventHandler<T1>(Expression<Func<IQuibbleHubClient, Func<T1, Task>>> eventSelector, Func<T1, Task> eventHandler) => 
            AddEventHandler(HubConnection.On(GetMethodName(eventSelector), eventHandler));

        protected void AddEventHandler<T1, T2>(Expression<Func<IQuibbleHubClient, Func<T1, T2, Task>>> eventSelector, Func<T1, T2, Task> eventHandler) =>
            AddEventHandler(HubConnection.On(GetMethodName(eventSelector), eventHandler));

        protected void AddFilteredEventHandler(Expression<Func<IQuibbleHubClient, Func<Guid, Task>>> eventSelector, Func<Task> eventHandler) =>
            AddEventHandler(HubConnection.On<Guid>(GetMethodName(eventSelector), id => id == Id ? eventHandler() : Task.CompletedTask));

        protected void AddFilteredEventHandler<T1>(Expression<Func<IQuibbleHubClient, Func<Guid, T1, Task>>> eventSelector, Func<T1, Task> eventHandler) =>
            AddEventHandler(HubConnection.On<Guid, T1>(GetMethodName(eventSelector), (id, t1) => id == Id ? eventHandler(t1) : Task.CompletedTask));

        protected void AddFilteredEventHandler<T1, T2>(Expression<Func<IQuibbleHubClient, Func<Guid, T1, Task>>> eventSelector, Func<T1, T2, Task> eventHandler) =>
            AddEventHandler(HubConnection.On<Guid, T1, T2>(GetMethodName(eventSelector), (id, t1, t2) => id == Id ? eventHandler(t1, t2) : Task.CompletedTask));

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

        protected Task OnUpdatedAsync() =>
            Updated?.Invoke() is null
                ? Task.CompletedTask
                : Updated.Invoke();

        public abstract int GetStateStamp();

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
