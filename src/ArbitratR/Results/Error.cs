namespace ArbitratR.Results
{
    /// <summary>
    /// Represents an error with a code and optional description.
    /// </summary>
    /// <param name="Code">The error code that uniquely identifies the error type.</param>
    /// <param name="Description">An optional description providing additional details about the error.</param>
    public record Error(string Code, string? Description = null)
    {
        /// <summary>
        /// Represents no error. Used to indicate a successful operation.
        /// </summary>
        public static readonly Error None = new(string.Empty);

        /// <summary>
        /// Represents an error indicating that a specified result value is null.
        /// </summary>
        public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

        /// <summary>
        /// Represents an error indicating that a specified condition was not met.
        /// </summary>
        public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", "The specified condition was not met.");
    }

    /// <summary>
    /// Represents a general problem error.
    /// </summary>
    /// <param name="Code">The error code that uniquely identifies the problem.</param>
    /// <param name="Description">A description providing details about the problem.</param>
    public record Problem(string Code, string? Description) : Error(Code, Description);

    /// <summary>
    /// Represents a validation error containing one or more validation failures.
    /// </summary>
    /// <param name="Errors">A dictionary of validation errors, where the key is the field name and the value is an array of error messages for that field.</param>
    public record ValidationError(IDictionary<string, string[]?> Errors) : Error("Error.Validation", "A validation error has occured.");

    /// <summary>
    /// Represents an error indicating that a requested resource was not found.
    /// </summary>
    /// <param name="Code">The error code that uniquely identifies the not found error.</param>
    /// <param name="Description">A description providing details about what was not found.</param>
    public record NotFound(string Code, string? Description) : Error(Code, Description);

    /// <summary>
    /// Represents an error indicating a conflict with the current state of a resource.
    /// </summary>
    /// <param name="Code">The error code that uniquely identifies the conflict.</param>
    /// <param name="Description">A description providing details about the conflict.</param>
    public record Conflict(string Code, string? Description) : Error(Code, Description);

    /// <summary>
    /// Represents an error indicating that access to a resource is forbidden.
    /// </summary>
    /// <param name="Code">The error code that uniquely identifies the forbidden error.</param>
    /// <param name="Description">A description providing details about why access is forbidden.</param>
    public record Forbidden(string Code, string? Description) : Error(Code, Description);

    /// <summary>
    /// Represents an error indicating that authentication is required or has failed.
    /// </summary>
    /// <param name="Code">The error code that uniquely identifies the unauthorised error.</param>
    /// <param name="Description">A description providing details about the authentication failure.</param>
    public record Unauthorised(string Code, string? Description) : Error(Code, Description);
    
    /// <summary>
    /// Represents an error indicating that the service is unavailable.
    /// </summary>
    /// <param name="Code">The error code that uniquely identifies the unauthorised error.</param>
    /// <param name="Description">A description providing details about the authentication failure.</param>
    public record ServiceUnavailable(string Code, string? Description) : Error(Code, Description);
}
