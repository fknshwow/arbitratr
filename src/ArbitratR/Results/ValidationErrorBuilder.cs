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
    
    public Result ToResult()
    {
        return Errors.Count > 0 ? Result.Failure(new ValidationError(Errors)) : Result.Success();
    }
}