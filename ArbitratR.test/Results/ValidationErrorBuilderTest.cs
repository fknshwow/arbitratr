using ArbitratR.Results;

namespace ArbitratR.test.Results;

public class ValidationErrorBuilderTest
{
    #region Create Tests

    [Fact]
    public void Create_ReturnsNewInstance()
    {
        // Act
        var builder = ValidationErrorBuilder.Create();

        // Assert
        Assert.NotNull(builder);
    }

    [Fact]
    public void Create_ReturnsEmptyBuilder()
    {
        // Act
        var builder = ValidationErrorBuilder.Create();
        var result = builder.ToResult();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_CalledMultipleTimes_ReturnsDifferentInstances()
    {
        // Act
        var builder1 = ValidationErrorBuilder.Create();
        var builder2 = ValidationErrorBuilder.Create();

        // Assert
        Assert.NotSame(builder1, builder2);
    }

    #endregion

    #region AddError Tests

    [Fact]
    public void AddError_WithValidError_AddsErrorToBuilder()
    {
        // Arrange
        var builder = ValidationErrorBuilder.Create();
        var error = new Error("TestCode", "Test description");

        // Act
        builder.AddError(error);
        var result = builder.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<ValidationError>(result.Error);
        var validationError = (ValidationError)result.Error;
        Assert.Single(validationError.Errors);
        Assert.True(validationError.Errors.ContainsKey("TestCode"));
        Assert.Equal(["Test description"], validationError.Errors["TestCode"]!);
    }

    [Fact]
    public void AddError_WithNullDescription_DoesNotAddError()
    {
        // Arrange
        var builder = ValidationErrorBuilder.Create();
        var error = new Error("TestCode");

        // Act
        builder.AddError(error);
        var result = builder.ToResult();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddError_SameCodeMultipleTimes_AppendsDescriptions()
    {
        // Arrange
        var builder = ValidationErrorBuilder.Create();
        var error1 = new Error("TestCode", "Description 1");
        var error2 = new Error("TestCode", "Description 2");

        // Act
        builder.AddError(error1);
        builder.AddError(error2);
        var result = builder.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        var validationError = (ValidationError)result.Error;
        Assert.Single(validationError.Errors);
        Assert.Equal(["Description 1", "Description 2"], validationError.Errors["TestCode"]!);
    }

    [Fact]
    public void AddError_DifferentCodes_CreatesMultipleEntries()
    {
        // Arrange
        var builder = ValidationErrorBuilder.Create();
        var error1 = new Error("Code1", "Description 1");
        var error2 = new Error("Code2", "Description 2");

        // Act
        builder.AddError(error1);
        builder.AddError(error2);
        var result = builder.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        var validationError = (ValidationError)result.Error;
        Assert.Equal(2, validationError.Errors.Count);
        Assert.Equal(["Description 1"], validationError.Errors["Code1"]!);
        Assert.Equal(["Description 2"], validationError.Errors["Code2"]!);
    }

    #endregion

    #region Merge Tests

    [Fact]
    public void Merge_WithEmptyBuilder_DoesNotChangeTarget()
    {
        // Arrange
        var builder1 = ValidationErrorBuilder.Create();
        var builder2 = ValidationErrorBuilder.Create();
        builder1.AddError(new Error("Code", "Description"));

        // Act
        builder1.Merge(builder2);
        var result = builder1.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        var validationError = (ValidationError)result.Error;
        Assert.Single(validationError.Errors);
    }

    [Fact]
    public void Merge_EmptyTargetWithNonEmptySource_AddsErrors()
    {
        // Arrange
        var builder1 = ValidationErrorBuilder.Create();
        var builder2 = ValidationErrorBuilder.Create();
        builder2.AddError(new Error("Code", "Description"));

        // Act
        builder1.Merge(builder2);
        var result = builder1.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        var validationError = (ValidationError)result.Error;
        Assert.Single(validationError.Errors);
        Assert.Equal(["Description"], validationError.Errors["Code"]!);
    }

    [Fact]
    public void Merge_WithOverlappingCodes_CombinesDescriptions()
    {
        // Arrange
        var builder1 = ValidationErrorBuilder.Create();
        var builder2 = ValidationErrorBuilder.Create();
        builder1.AddError(new Error("Code", "Description 1"));
        builder2.AddError(new Error("Code", "Description 2"));

        // Act
        builder1.Merge(builder2);
        var result = builder1.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        var validationError = (ValidationError)result.Error;
        Assert.Single(validationError.Errors);
        Assert.Equal(["Description 1", "Description 2"], validationError.Errors["Code"]!);
    }

    [Fact]
    public void Merge_WithDifferentCodes_AddsAllErrors()
    {
        // Arrange
        var builder1 = ValidationErrorBuilder.Create();
        var builder2 = ValidationErrorBuilder.Create();
        builder1.AddError(new Error("Code1", "Description 1"));
        builder2.AddError(new Error("Code2", "Description 2"));

        // Act
        builder1.Merge(builder2);
        var result = builder1.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        var validationError = (ValidationError)result.Error;
        Assert.Equal(2, validationError.Errors.Count);
        Assert.Equal(["Description 1"], validationError.Errors["Code1"]!);
        Assert.Equal(["Description 2"], validationError.Errors["Code2"]!);
    }

    [Fact]
    public void Merge_BothBuildersEmpty_ResultIsSuccess()
    {
        // Arrange
        var builder1 = ValidationErrorBuilder.Create();
        var builder2 = ValidationErrorBuilder.Create();

        // Act
        builder1.Merge(builder2);
        var result = builder1.ToResult();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Merge_MultipleBuilders_CombinesAllErrors()
    {
        // Arrange
        var builder1 = ValidationErrorBuilder.Create();
        var builder2 = ValidationErrorBuilder.Create();
        var builder3 = ValidationErrorBuilder.Create();
        builder1.AddError(new Error("Code1", "Description 1"));
        builder2.AddError(new Error("Code2", "Description 2"));
        builder3.AddError(new Error("Code1", "Description 3"));

        // Act
        builder1.Merge(builder2);
        builder1.Merge(builder3);
        var result = builder1.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        var validationError = (ValidationError)result.Error;
        Assert.Equal(2, validationError.Errors.Count);
        Assert.Equal(["Description 1", "Description 3"], validationError.Errors["Code1"]!);
        Assert.Equal(["Description 2"], validationError.Errors["Code2"]!);
    }

    #endregion

    #region ToResult Tests

    [Fact]
    public void ToResult_NoErrors_ReturnsSuccess()
    {
        // Arrange
        var builder = ValidationErrorBuilder.Create();

        // Act
        var result = builder.ToResult();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void ToResult_WithErrors_ReturnsFailure()
    {
        // Arrange
        var builder = ValidationErrorBuilder.Create();
        builder.AddError(new Error("Code", "Description"));

        // Act
        var result = builder.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void ToResult_WithErrors_ReturnsValidationError()
    {
        // Arrange
        var builder = ValidationErrorBuilder.Create();
        builder.AddError(new Error("Code", "Description"));

        // Act
        var result = builder.ToResult();

        // Assert
        Assert.IsType<ValidationError>(result.Error);
    }

    [Fact]
    public void ToResult_CalledMultipleTimes_ReturnsSameResult()
    {
        // Arrange
        var builder = ValidationErrorBuilder.Create();
        builder.AddError(new Error("Code", "Description"));

        // Act
        var result1 = builder.ToResult();
        var result2 = builder.ToResult();

        // Assert
        Assert.True(result1.IsFailure);
        Assert.True(result2.IsFailure);
        Assert.Equal(result1.Error, result2.Error);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void ComplexScenario_MixedOperations_WorksCorrectly()
    {
        // Arrange
        var builder1 = ValidationErrorBuilder.Create();
        var builder2 = ValidationErrorBuilder.Create();

        // Act - Add multiple errors with same and different codes
        builder1.AddError(new Error("Email", "Email is required"));
        builder1.AddError(new Error("Email", "Email format is invalid"));
        builder1.AddError(new Error("Password", "Password is required"));

        builder2.AddError(new Error("Email", "Email already exists"));
        builder2.AddError(new Error("Username", "Username is required"));

        builder1.Merge(builder2);
        var result = builder1.ToResult();

        // Assert
        Assert.True(result.IsFailure);
        var validationError = (ValidationError)result.Error;
        Assert.Equal(3, validationError.Errors.Count);
        Assert.Equal(["Email is required", "Email format is invalid", "Email already exists"], validationError.Errors["Email"]!);
        Assert.Equal(["Password is required"], validationError.Errors["Password"]!);
        Assert.Equal(["Username is required"], validationError.Errors["Username"]!);
    }

    [Fact]
    public void ValidationError_HasCorrectDefaultValues()
    {
        // Arrange
        var builder = ValidationErrorBuilder.Create();
        builder.AddError(new Error("Code", "Description"));

        // Act
        var result = builder.ToResult();
        var validationError = result.Error;

        // Assert
        Assert.Equal("Error-Validation", validationError?.Code);
        Assert.Equal("A validation error has occured.", validationError?.Description);
    }

    #endregion
}