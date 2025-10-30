namespace ArbitratR.CQRS
{
    /// <summary>
    /// Marker interface for commands that do not return a result value.
    /// Commands represent actions or intentions to change state in the system.
    /// </summary>
    public interface ICommand;

    /// <summary>
    /// Marker interface for commands that return a result value.
    /// Commands represent actions or intentions to change state in the system.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value returned by the command.</typeparam>
    public interface ICommand<TResult>;
}
