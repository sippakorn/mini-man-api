using MiniMan.Models.Enums;

namespace MiniMan.Models;

/// <summary>
/// Represents a work order in the system
/// </summary>
public class WorkOrder
{
    /// <summary>
    /// Unique identifier for the work order
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Order number for tracking
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Description of the work to be performed
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the work order was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Due date for completing the work order
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Name or identifier of the person assigned to the work order
    /// </summary>
    public string AssignedTo { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the work order
    /// </summary>
    public WorkOrderStatus Status { get; set; }

    /// <summary>
    /// Category of the work order
    /// </summary>
    public string Category { get; set; } = string.Empty;
}
