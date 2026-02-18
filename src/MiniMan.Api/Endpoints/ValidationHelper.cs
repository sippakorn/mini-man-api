using FluentValidation.Results;

namespace MiniMan.Api.Endpoints;

/// <summary>
/// Helper class for validation operations
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Converts FluentValidation results to a dictionary suitable for ValidationProblem
    /// </summary>
    public static IDictionary<string, string[]> ToErrorDictionary(ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }
}
