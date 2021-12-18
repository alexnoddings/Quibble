using Microsoft.EntityFrameworkCore;
using Quibble.Common.Entities;

namespace Quibble.Server.Core;

internal static class EntityQueryableExtensions
{
	/// <summary>
	///     Asynchronously finds a <typeparamref name="TEntity"/> with the given <paramref name="id"/>, or
	///     <see langword="null"/> if it does not exist.
	/// </summary>
	/// <typeparam name="TId">The type of Id which <typeparamref name="TEntity"/>s have.</typeparam>
	/// <typeparam name="TEntity">The type of the elements of <paramref name="source" />.</typeparam>
	/// <param name="source">An <see cref="IQueryable{T}" /> to find the entity in.</param>
	/// <param name="id">The id to search for.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	///     The task result contains the found <typeparamref name="TEntity"/>,
	///     or <see langword="null"/> if no such element is found.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null" />.</exception>
	/// <exception cref="OperationCanceledException">If the <see cref="CancellationToken"/> is cancelled.</exception>
	public static Task<TEntity?> FindAsync<TId, TEntity>(this IQueryable<TEntity> source, TId id, CancellationToken cancellationToken = default)
		where TId : IEquatable<TId>
		where TEntity : IEntity<TId> =>
		source.FirstOrDefaultAsync(entity => entity.Id.Equals(id), cancellationToken);
}
