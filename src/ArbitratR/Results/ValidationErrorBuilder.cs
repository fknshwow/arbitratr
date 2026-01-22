namespace ArbitratR.Results;

public class ValidationErrorBuilder
{
    private Dictionary<string, string[]?> Errors { get; }
    
    private ValidationErrorBuilder()
    {
        Errors = new Dictionary<string, string[]?>();
    }
    
    public static ValidationErrorBuilder Create()
    {
        return new ValidationErrorBuilder();
    }
    
    public ValidationErrorBuilder AddError(string errorCode, string errorDetails)
    {
        if (Errors.TryGetValue(errorCode, out var existingErrors))
        {
            if (existingErrors is null)
            {
                Errors[errorCode] = [errorDetails];
            }
            else
            {
                string[] updatedErrors = [..existingErrors, errorDetails];
                Errors[errorCode] = updatedErrors;
            }
        }
        else Errors.TryAdd(errorCode, [errorDetails]);

        return this;
    }
    
    public Result ToResult()
    {
        return Errors.Count > 0 ? Result.Failure(new ValidationError(Errors)) : Result.Success();
    }
}