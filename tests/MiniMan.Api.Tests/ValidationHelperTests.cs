using FluentAssertions;
using FluentValidation.Results;
using MiniMan.Api.Endpoints;

namespace MiniMan.Api.Tests;

/// <summary>
/// Unit tests for ValidationHelper
/// </summary>
public class ValidationHelperTests
{
    [Fact]
    public void ToErrorDictionary_WithSingleError_ReturnsCorrectDictionary()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("PropertyName", "Error message")
        });

        // Act
        var result = ValidationHelper.ToErrorDictionary(validationResult);

        // Assert
        result.Should().ContainKey("PropertyName");
        result["PropertyName"].Should().HaveCount(1);
        result["PropertyName"][0].Should().Be("Error message");
    }

    [Fact]
    public void ToErrorDictionary_WithMultipleErrorsOnSameProperty_GroupsErrorsTogether()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("PropertyName", "First error"),
            new ValidationFailure("PropertyName", "Second error")
        });

        // Act
        var result = ValidationHelper.ToErrorDictionary(validationResult);

        // Assert
        result.Should().ContainKey("PropertyName");
        result["PropertyName"].Should().HaveCount(2);
        result["PropertyName"].Should().Contain("First error");
        result["PropertyName"].Should().Contain("Second error");
    }

    [Fact]
    public void ToErrorDictionary_WithMultipleErrorsOnDifferentProperties_ReturnsMultipleKeys()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Property1", "Error for property 1"),
            new ValidationFailure("Property2", "Error for property 2"),
            new ValidationFailure("Property3", "Error for property 3")
        });

        // Act
        var result = ValidationHelper.ToErrorDictionary(validationResult);

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainKey("Property1");
        result.Should().ContainKey("Property2");
        result.Should().ContainKey("Property3");
        result["Property1"][0].Should().Be("Error for property 1");
        result["Property2"][0].Should().Be("Error for property 2");
        result["Property3"][0].Should().Be("Error for property 3");
    }

    [Fact]
    public void ToErrorDictionary_WithEmptyValidationResult_ReturnsEmptyDictionary()
    {
        // Arrange
        var validationResult = new ValidationResult();

        // Act
        var result = ValidationHelper.ToErrorDictionary(validationResult);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToErrorDictionary_WithEmptyPropertyName_HandlesCorrectly()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("", "General error message")
        });

        // Act
        var result = ValidationHelper.ToErrorDictionary(validationResult);

        // Assert
        result.Should().ContainKey("");
        result[""][0].Should().Be("General error message");
    }

    [Fact]
    public void ToErrorDictionary_WithMixedPropertyNamesAndMultipleErrors_GroupsCorrectly()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("OrderNumber", "Order number is required"),
            new ValidationFailure("OrderNumber", "Order number must not exceed 50 characters"),
            new ValidationFailure("Description", "Description is required"),
            new ValidationFailure("DueDate", "Due date must be in the future"),
            new ValidationFailure("AssignedTo", "Assigned to is required")
        });

        // Act
        var result = ValidationHelper.ToErrorDictionary(validationResult);

        // Assert
        result.Should().HaveCount(4);
        result["OrderNumber"].Should().HaveCount(2);
        result["OrderNumber"].Should().Contain("Order number is required");
        result["OrderNumber"].Should().Contain("Order number must not exceed 50 characters");
        result["Description"].Should().HaveCount(1);
        result["Description"][0].Should().Be("Description is required");
        result["DueDate"].Should().HaveCount(1);
        result["AssignedTo"].Should().HaveCount(1);
    }

    [Fact]
    public void ToErrorDictionary_ReturnsIDictionaryType()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("PropertyName", "Error message")
        });

        // Act
        var result = ValidationHelper.ToErrorDictionary(validationResult);

        // Assert
        result.Should().BeAssignableTo<IDictionary<string, string[]>>();
    }
}
