using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quibble.Common.Api;
using Quibble.Server.Core.Domain;
using System.Runtime.CompilerServices;

namespace Quibble.Sync.InMemory.Client.Contexts;

internal abstract class BaseSyncContext<TLogic> : BaseSyncContext where TLogic : BaseLogic
{
	protected BaseSyncContext(ILogger<BaseSyncContext> logger, SyncContext parent)
		: base(logger, parent)
	{
	}

	protected Task InvokeAsync<TResult>(Func<TLogic, Task<ApiResponse<TResult>>> func, [CallerMemberName] string caller = "") =>
		InvokeApiAsync(async service => await func(service), caller);

	protected Task InvokeAsync(Func<TLogic, Task<ApiResponse>> func, [CallerMemberName] string caller = "") =>
		InvokeApiAsync(func, caller);

	private Task InvokeApiAsync(Func<TLogic, Task<ApiResponse>> func, string caller)
	{
		async Task<ApiResponse> Inner(TLogic logic)
		{
			var result = await func(logic);
			if (!result.WasSuccessful)
				Logger.LogDebug("{Method} result was unsuccessful: {Error}.", caller, result.Error);
			else
				Logger.LogDebug("{Method} result was successful.", caller);
			return result;
		}
		return InvokeAsync<ApiResponse>(Inner, caller);
	}

	protected async Task<TResult> InvokeAsync<TResult>(Func<TLogic, Task<TResult>> func, string caller = "")
	{
		var scope = Parent.ScopeFactory.CreateAsyncScope();
		var logicService = scope.ServiceProvider.GetRequiredService<TLogic>();

		Logger.LogDebug("Invoking {Method} as {UserId}.", caller, Context.UserId);
		var result = await func(logicService);
		await scope.DisposeAsync();

		return result;
	}
}
