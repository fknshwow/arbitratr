using ArbitratR.Results;

namespace ArbitratR.CQRS
{
    /// <summary>
    /// Defines a handler for queries that return a result value.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to handle, which must implement <see cref="IQuery{TResult}"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by the query.</typeparam>
    public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Handles the specified query asynchronously.
        /// </summary>
        /// <param name="query">The query to handle.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="Result{TValue}"/> with the result value or an error.</returns>
        Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
