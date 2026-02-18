using MiniMan.Models.Enums;

namespace MiniMan.Models;

/// <summary>
/// Represents a maintenance notification in the system
/// </summary>
public class MaintenanceNotification
{
    /// <summary>
    /// Unique identifier for the maintenance notification
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Title of the maintenance notification
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the maintenance notification
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the notification was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Date and time when the maintenance is scheduled
    /// </summary>
    public DateTime ScheduledDate { get; set; }

    /// <summary>
    /// Current status of the maintenance notification
    /// </summary>
    public MaintenanceStatus Status { get; set; }

    /// <summary>
    /// Priority level of the maintenance notification
    /// </summary>
    public Priority Priority { get; set; }
}
