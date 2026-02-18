using FluentValidation;
using MiniMan.Models;
using MiniMan.Models.Enums;

namespace MiniMan.Api.Validators;

/// <summary>
/// Validator for MaintenanceNotification model
/// </summary>
public class MaintenanceNotificationValidator : AbstractValidator<MaintenanceNotification>
{
    public MaintenanceNotificationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.ScheduledDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Scheduled date must be in the future")
            .When(x => x.Status == MaintenanceStatus.Pending);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value");
    }
}
