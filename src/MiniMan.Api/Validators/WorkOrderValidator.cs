using FluentValidation;
using MiniMan.Models;

namespace MiniMan.Api.Validators;

/// <summary>
/// Validator for WorkOrder model
/// </summary>
public class WorkOrderValidator : AbstractValidator<WorkOrder>
{
    public WorkOrderValidator()
    {
        RuleFor(x => x.OrderNumber)
            .NotEmpty().WithMessage("Order number is required")
            .MaximumLength(50).WithMessage("Order number must not exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.DueDate)
            .Must(date => date > DateTime.UtcNow).WithMessage("Due date must be in the future");

        RuleFor(x => x.AssignedTo)
            .NotEmpty().WithMessage("Assigned to is required")
            .MaximumLength(100).WithMessage("Assigned to must not exceed 100 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters");
    }
}
