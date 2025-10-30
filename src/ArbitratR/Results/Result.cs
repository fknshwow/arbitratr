namespace ArbitratR.Results
{
    /// <summary>
    /// Represents the result of an operation, encapsulating either a success or failure state with an optional error.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="error">The error associated with a failed operation, or <see cref="Error.None"/> for success.</param>
        /// <exception cref="ArgumentException">Thrown when isSuccess is true but error is not <see cref="Error.None"/>, or when isSuccess is false but error is <see cref="Error.None"/>.</exception>
        protected Result(bool isSuccess, Error? error)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Gets the error associated with the failed operation, or <c>null</c> if the operation was successful.
        /// </summary>
        public Error? Error { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>A <see cref="Result"/> representing a successful operation.</returns>
        public static Result Success() => new(true, Error.None);

        /// <summary>
        /// Creates a failed result with the specified error.
        /// </summary>
        /// <param name="error">The error describing why the operation failed.</param>
        /// <returns>A <see cref="Result"/> representing a failed operation.</returns>
        public static Result Failure(Error error) => new(false, error);

        /// <summary>
        /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result"/>.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        public static implicit operator Result(Error error) => Failure(error);

        /// <summary>
        /// Matches the result to one of two functions based on whether the operation was successful or failed.
        /// </summary>
        /// <typeparam name="TResult">The type of the value to return.</typeparam>
        /// <param name="success">The function to execute if the operation was successful.</param>
        /// <param name="failure">The function to execute if the operation failed, receiving the error.</param>
        /// <returns>The result of executing either the success or failure function.</returns>
        public TResult Match<TResult>(
            Func<TResult> success,
            Func<Error, TResult> failure) =>
            IsSuccess ? success() : failure(Error!);
    }

    /// <summary>
    /// Represents the result of an operation that returns a value, encapsulating either a success state with a value or a failure state with an error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value returned by a successful operation.</typeparam>
    public class Result<TValue> : Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TValue}"/> class representing a successful operation.
        /// </summary>
        /// <param name="value">The value returned by the successful operation.</param>
        private Result(TValue value) : base(true, Error.None)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TValue}"/> class representing a failed operation.
        /// </summary>
        /// <param name="error">The error describing why the operation failed.</param>
        private Result(Error error) : base(false, error)
        {
            Value = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TValue}"/> class with the specified value, success state, and error.
        /// </summary>
        /// <param name="value">The value returned by the operation.</param>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="error">The error associated with a failed operation.</param>
        public Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value returned by a successful operation, or the default value if the operation failed.
        /// </summary>
        public TValue? Value { get; }

        /// <summary>
        /// Creates a successful result with the specified value.
        /// </summary>
        /// <param name="value">The value returned by the successful operation.</param>
        /// <returns>A <see cref="Result{TValue}"/> representing a successful operation with the specified value.</returns>
        public static Result<TValue> Success(TValue value) => new(value);

        /// <summary>
        /// Implicitly converts a value to a successful <see cref="Result{TValue}"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator Result<TValue>(TValue value) => new(value);

        /// <summary>
        /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result{TValue}"/>.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        public static implicit operator Result<TValue>(Error error) => new(error);

        /// <summary>
        /// Matches the result to one of two functions based on whether the operation was successful or failed.
        /// </summary>
        /// <typeparam name="TResult">The type of the value to return.</typeparam>
        /// <param name="success">The function to execute if the operation was successful, receiving the value.</param>
        /// <param name="failure">The function to execute if the operation failed, receiving the error.</param>
        /// <returns>The result of executing either the success or failure function.</returns>
        public TResult Match<TResult>(
            Func<TValue, TResult> success,
            Func<Error, TResult> failure) =>
            IsSuccess ? success(Value!) : failure(Error!);
    }
}
