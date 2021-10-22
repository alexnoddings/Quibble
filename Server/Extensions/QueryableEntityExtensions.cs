using Microsoft.EntityFrameworkCore;
using Quibble.Shared.Entities;

namespace Quibble.Server.Extensions;

public static class QueryableEntityExtensions
{
    /// <summary>
    ///     Asynchronously finds an <typeparamref name="TEntity"/> with the given <paramref name="id"/>, or
    ///     <c><see langword="default" />(<typeparamref name="TEntity"/>)</c> if it does not exist;
    ///     this method throws an exception if more than one element is found with the <paramref name="id"/>.
    /// </summary>
    /// <typeparam name="TId"> The type of Id which <typeparamref name="TEntity"/>s have.</typeparam>
    /// <typeparam name="TEntity"> The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source"> An <see cref="IQueryable{T}" /> to find the entity in.</param>
    /// <param name="id"> The id to search for.</param>
    /// <param name="cancellationToken"> A <see cref="CancellationToken" /> to observe while waiting for the task to complete. </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the found <typeparamref name="TEntity"/>,
    ///     or <c><see langword="default" />(<typeparamref name="TEntity"/>)</c> if no such element is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     More than one <typeparamref name="TEntity"/> in <paramref name="source"/> has the given <paramref name="id"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken"/> is cancelled.</exception>
    public static Task<TEntity?> FindAsync<TId, TEntity>(this IQueryable<TEntity> source, TId id, CancellationToken cancellationToken = default)
        where TId : IEquatable<TId>
        where TEntity : IEntity<TId> =>
        source.SingleOrDefaultAsync(entity => entity.Id.Equals(id), cancellationToken);
}
