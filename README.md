# ArbitratR

A comprehensive CQRS (Command Query Responsibility Segregation) library for .NET applications that enforces the Result pattern for robust error handling and provides seamless dependency injection integration.

## Features

- 🎯 **Clean CQRS Implementation** - Separate command and query handling with clear interfaces
- ✅ **Enforced Result Pattern** - All operations return `Result<T>` for consistent error handling
- 🔧 **Flexible Configuration** - Multiple registration strategies for handlers
- 📦 **Dependency Injection Ready** - Built-in support for Microsoft.Extensions.DependencyInjection
- 🚀 **High Performance** - Lightweight with minimal overhead
- 📚 **Rich Error Types** - Comprehensive error hierarchy for different failure scenarios

## Installation

```bash
dotnet add package ArbitratR
```

## Quick Start

### 1. Register ArbitratR Services

```csharp
using ArbitratR.CQRS;

// In Program.cs or Startup.cs
services.AddArbitratR(config =>
{
    // Register handlers from current assembly
    config.AddHandlers();
    
    // Or register from specific assemblies
    config.AddHandlers(typeof(MyHandler).Assembly, typeof(AnotherHandler).Assembly);
});
```

### 2. Define Commands and Handlers

```csharp
using ArbitratR.CQRS;
using ArbitratR.Results;

// Command without return value
public record CreateUserCommand(string Name, string Email) : ICommand;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public async Task<Result> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrEmpty(command.Email))
            return new ValidationError(new Dictionary<string, string[]?> 
            { 
                ["Email"] = ["Email is required"] 
            });

        // Business logic
        try
        {
            // Create user logic here
            return Result.Success();
        }
        catch (Exception ex)
        {
            return new Problem("User-CreationFailed", ex.Message);
        }
    }
}

// Command with return value
public record GetUserByIdCommand(int Id) : ICommand<User>;

public class GetUserByIdCommandHandler : ICommandHandler<GetUserByIdCommand, User>
{
    public async Task<Result<User>> HandleAsync(GetUserByIdCommand command, CancellationToken cancellationToken)
    {
        var user = await FindUserAsync(command.Id);
        
        return user != null 
            ? Result<User>.Success(user)
            : new NotFound("User-NotFound", $"User with ID {command.Id} was not found");
    }
}
```

### 3. Define Queries and Handlers

```csharp
// Query
public record GetUsersQuery(int PageSize, int PageNumber) : IQuery<IEnumerable<User>>;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IEnumerable<User>>
{
    public async Task<Result<IEnumerable<User>>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
    {
        if (query.PageSize <= 0)
            return new ValidationError(new Dictionary<string, string[]?> 
            { 
                ["PageSize"] = ["Page size must be greater than 0"] 
            });

        var users = await GetPagedUsersAsync(query.PageSize, query.PageNumber);
        return Result<IEnumerable<User>>.Success(users);
    }
}
```

### 4. Use in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ICommandHandler<CreateUserCommand> _createUserHandler;
    private readonly IQueryHandler<GetUsersQuery, IEnumerable<User>> _getUsersHandler;

    public UsersController(
        ICommandHandler<CreateUserCommand> createUserHandler,
        IQueryHandler<GetUsersQuery, IEnumerable<User>> getUsersHandler)
    {
        _createUserHandler = createUserHandler;
        _getUsersHandler = getUsersHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        var result = await _createUserHandler.HandleAsync(command, CancellationToken.None);
        
        return result.Match(
            success: () => Ok(),
            failure: error => error switch
            {
                ValidationError validation => BadRequest(validation.Errors),
                _ => StatusCode(500, error.Description)
            });
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
    {
        var query = new GetUsersQuery(pageSize, pageNumber);
        var result = await _getUsersHandler.HandleAsync(query, CancellationToken.None);
        
        return result.Match(
            success: users => Ok(users),
            failure: error => BadRequest(error.Description));
    }
}
```

## Configuration Options

```csharp
services.AddArbitratR(config =>
{
    // Register all handlers from calling assembly
    config.AddHandlers();
    
    // Register from specific assemblies
    config.AddHandlers(typeof(UserHandler).Assembly, typeof(OrderHandler).Assembly);
    
    // Register only commands or queries
    config.AddCommandHandlers(typeof(Commands).Assembly);
    config.AddQueryHandlers(typeof(Queries).Assembly);
});
```

## Result Pattern

ArbitratR enforces the Result pattern for all operations, providing consistent error handling across your application.

### Result Types

```csharp
// Basic result (success/failure)
Result result = Result.Success();
Result failure = Result.Failure(new Problem("User-OperationFailed", "Description"));

// Result with value
Result<User> userResult = Result<User>.Success(user);
Result<User> notFound = new NotFound("User-NotFound", "User not found");
```

### Built-in Error Types

```csharp
// Validation errors
var validationError = new ValidationError(new Dictionary<string, string[]?> 
{ 
    ["Email"] = ["Email is required", "Email format is invalid"],
    ["Age"] = ["Age must be greater than 0"]
});

// Not found errors
var notFound = new NotFound("User-NotFound", "The requested user was not found");

// Conflict errors
var conflict = new Conflict("User-EmailInUse", "A user with this email already exists");

// Authorization errors
var unauthorized = new Unauthorised("User-InvalidToken", "The provided token is invalid");
var forbidden = new Forbidden("User-InsufficientPermissions", "You don't have permission to perform this action");

// General problems
var problem = new Problem("User-ExternalServiceError", "The external service is currently unavailable");
```

### Custom Error Classes

Create domain-specific error classes for better organization and reusability:

```csharp
public static class UserErrors
{
    public static readonly NotFound NotFound = new("User-NotFound", "The user could not be found.");
    public static readonly Problem IncorrectPassword = new("User-IncorrectPassword", "The password supplied was incorrect.");
    public static readonly Forbidden NotAuthorised = new("User-NotAuthorised", "You do not have permission to perform this action.");
    public static readonly Unauthorised NotAuthenticated = new("User-NotAuthenticated", "User could not be authenticated.");
    
    // Dynamic errors with parameters
    public static Problem EmailInUse(string email) => new("User-EmailInUse", $"A user already exists for the email '{email}'");
}

// Usage in handlers
public async Task<Result<User>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
{
    if (await EmailExistsAsync(command.Email))
        return UserErrors.EmailInUse(command.Email);
    
    // Create user logic...
    return Result<User>.Success(user);
}
```

### Pattern Matching with Results

```csharp
var result = await handler.HandleAsync(command, cancellationToken);

// Simple pattern matching
return result.Match(
    success: () => Ok(),
    failure: error => BadRequest(error.Description)
);

// Advanced error handling
return result.Match(
    success: () => Ok(),
    failure: error => error switch
    {
        ValidationError validation => BadRequest(validation.Errors),
        NotFound notFound => NotFound(notFound.Description),
        Unauthorised _ => Unauthorized(),
        Forbidden _ => Forbid(),
        _ => StatusCode(500, "An unexpected error occurred")
    }
);

// With value results
var userResult = await getUserHandler.HandleAsync(command, cancellationToken);
return userResult.Match(
    success: user => Ok(user),
    failure: error => HandleError(error)
);
```

### Implicit Conversions

```csharp
public async Task<Result<User>> GetUserAsync(int id)
{
    var user = await FindUserAsync(id);
    if (user == null)
        return UserErrors.NotFound; // Implicit conversion
    
    return user; // Implicit conversion to Result<User>.Success(user)
}
```

## Best Practices

### 1. Command/Query Separation
- **Commands** modify state and may or may not return data
- **Queries** only read data and never modify state
- Keep handlers focused on a single responsibility

### 2. Error Handling
- Use specific error types (`ValidationError`, `NotFound`, etc.)
- Provide meaningful error codes and descriptions
- Handle errors consistently using pattern matching

### 3. Validation
- Validate input in handlers, not in commands/queries
- Return `ValidationError` for input validation failures
- Use meaningful field names in validation errors

### 4. Handler Organization
- Group related handlers in the same assembly/namespace
- Use descriptive names for commands, queries, and handlers
- Keep handlers lightweight and delegate complex logic to domain services

### 5. Dependency Injection
- Register handlers using the provided configuration methods
- Prefer constructor injection for handler dependencies
- Use scoped lifetime for handlers (default behavior)

## License

MIT