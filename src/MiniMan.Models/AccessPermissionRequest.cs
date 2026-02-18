using MiniMan.Models.Enums;

namespace MiniMan.Models;

/// <summary>
/// Represents an access permission request in the system
/// </summary>
public class AccessPermissionRequest
{
    /// <summary>
    /// Unique identifier for the permission request
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name or identifier of the person requesting permission
    /// </summary>
    public string RequestedBy { get; set; } = string.Empty;

    /// <summary>
    /// Name of the resource for which permission is requested
    /// </summary>
    public string ResourceName { get; set; } = string.Empty;

    /// <summary>
    /// Type of permission being requested
    /// </summary>
    public PermissionType PermissionType { get; set; }

    /// <summary>
    /// Date and time when the request was made
    /// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// Date and time when the request was approved (null if not yet approved)
    /// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Current status of the permission request
    /// </summary>
    public PermissionStatus Status { get; set; }

    /// <summary>
    /// Reason for the permission request
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
