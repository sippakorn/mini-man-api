using FluentValidation;
using MiniMan.Models;

namespace MiniMan.Api.Validators;

/// <summary>
/// Validator for AccessPermissionRequest model
/// </summary>
public class AccessPermissionRequestValidator : AbstractValidator<AccessPermissionRequest>
{
    public AccessPermissionRequestValidator()
    {
        RuleFor(x => x.RequestedBy)
            .NotEmpty().WithMessage("Requested by is required")
            .MaximumLength(100).WithMessage("Requested by must not exceed 100 characters");

        RuleFor(x => x.ResourceName)
            .NotEmpty().WithMessage("Resource name is required")
            .MaximumLength(200).WithMessage("Resource name must not exceed 200 characters");

        RuleFor(x => x.PermissionType)
            .IsInEnum().WithMessage("Invalid permission type value");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(1000).WithMessage("Reason must not exceed 1000 characters");

        RuleFor(x => x.ApprovedDate)
            .GreaterThan(x => x.RequestDate).WithMessage("Approved date must be after request date")
            .When(x => x.ApprovedDate.HasValue);
    }
}
