namespace ArbitratR.Results;

public class ValidationErrorBuilder
{
    private Dictionary<string, string[]?> Errors { get; }
    
    private ValidationErrorBuilder()
    {
        Errors = new Dictionary<string, string[]?>();
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="ValidationErrorBuilder"/>.
    /// </summary>
    /// <returns>a new instance of <see cref="ValidationErrorBuilder"/>.</returns>
    public static ValidationErrorBuilder Create()
    {
        return new ValidationErrorBuilder();
    }

    /// <summary>
    /// Merges another <see cref="ValidationErrorBuilder"/> into this builder, with errors inside the two builders merged.
    /// </summary>
    /// <param name="builder">An instance of <see cref="ValidationErrorBuilder"/> to be merged in.</param>
    public void Merge(ValidationErrorBuilder builder)
    {
        foreach (var (key, value) in builder.Errors)
        {
            if (Errors.TryGetValue(key, out var existingErrors))
            {
                if (existingErrors is null)
                {
                    Errors[key] = value;
                }
                else if (value is not null)
                {
                    string[] updatedErrors = [..existingErrors, ..value];
                    Errors[key] = updatedErrors;
                }
            }
            else
            {
                Errors.TryAdd(key, value);
            }
        }
    }
    
    /// <summary>
    /// Adds an <see cref="Error"/> to the dictionary of errors.
    /// </summary>
    /// <remarks>
    /// The error code is the key of the error dictionary.
    /// If the error code already exists in the dictionary, the error description is appended to the existing array of descriptions.
    /// Otherwise, a new entry is created with the error code and an array containing the error description.
    /// </remarks>
    /// <param name="error">The <see cref="Error"/> to be added to the dictionary of errors.</param>
    public void AddError(Error error)
    {
        if (error.Description is null) return;
        
        if (Errors.TryGetValue(error.Code, out var existingErrors))
        {
            if (existingErrors is null)
            {
                Errors[error.Code] = [error.Description];
            }
            else
            {
                string[] updatedErrors = [..existingErrors, error.Description];
                Errors[error.Code] = updatedErrors;
            }
        }
        else Errors.TryAdd(error.Code, [error.Description]);
    }
    
    /// <summary>
    /// Converts the collected errors into a <see cref="Result"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> of success if there is no error in the builder.
    /// Otherwise, a <see cref="Result"/> of failure containing the error dictionary.
    /// </returns>
    public Result ToResult()
    {
        return Errors.Count > 0 ? Result.Failure(new ValidationError(Errors)) : Result.Success();
    }
}