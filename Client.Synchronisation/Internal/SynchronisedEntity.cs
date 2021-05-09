using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Quibble.Client.Sync.Entities;

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

        protected void AddEventHandler(IDisposable handler) => 
	        EventHandlers.Add(handler);

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
