using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quibble.Sync.SignalR.Shared;
using System.Linq.Expressions;

namespace Quibble.Sync.SignalR.Client.Contexts;

internal abstract class BaseSignalrSyncContext : IDisposable
{
	protected ILogger<BaseSignalrSyncContext> Logger { get; }

	private HubConnection? _hubConnection;
	protected HubConnection HubConnection => _hubConnection ?? throw new ObjectDisposedException(nameof(HubConnection));

	public BaseSignalrSyncContext(ILogger<BaseSignalrSyncContext> logger, HubConnection hubConnection)
	{
		Logger = logger;
		_hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
	}

	protected void Bind(Expression<Func<ISignalrEvents, Func<Task>>> eventExpression, Func<Func<Task>?> eventHandlerGetter)
	{
		var eventName = eventExpression.GetEventName();
		var executor = () => ExecuteEventHandler(eventName, () => eventHandlerGetter()?.Invoke() ?? Task.CompletedTask);
		HubConnection.On(eventName, executor);
	}

	protected void Bind<T1>(Expression<Func<ISignalrEvents, Func<T1, Task>>> eventExpression, Func<Func<T1, Task>?> eventHandlerGetter)
	{
		var eventName = eventExpression.GetEventName();
		var executor = (T1 arg1) => ExecuteEventHandler(eventName, () => eventHandlerGetter()?.Invoke(arg1) ?? Task.CompletedTask);
		HubConnection.On(eventName, executor);
	}

	protected void Bind<T1, T2>(Expression<Func<ISignalrEvents, Func<T1, T2, Task>>> eventExpression, Func<Func<T1, T2, Task>?> eventHandlerGetter)
	{
		var eventName = eventExpression.GetEventName();
		var executor = (T1 arg1, T2 arg2) => ExecuteEventHandler(eventName, () => eventHandlerGetter()?.Invoke(arg1, arg2) ?? Task.CompletedTask);
		HubConnection.On(eventName, executor);
	}

	private async Task ExecuteEventHandler(string eventName, Func<Task> eventFunc)
	{
		try
		{
			await eventFunc();
		}
		catch (Exception e)
		{
			Logger.LogError(e, "Exception while handling event {Event}.", eventName);
			throw;
		}
	}

	public void Dispose()
	{
		_hubConnection = null;
	}
}
