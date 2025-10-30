namespace ArbitratR.CQRS
{
    /// <summary>
    /// Marker interface for queries that return a result value.
    /// Queries represent requests for data without changing system state.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value returned by the query.</typeparam>
    public interface IQuery<TResult>;
}
