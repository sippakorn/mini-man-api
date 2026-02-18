using Microsoft.EntityFrameworkCore;
using MiniMan.Models;

namespace MiniMan.Api.Data;

/// <summary>
/// Database context for the MiniMan API with temporal table support
/// </summary>
public class MiniManDbContext : DbContext
{
    public MiniManDbContext(DbContextOptions<MiniManDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Maintenance notifications with temporal history
    /// </summary>
    public DbSet<MaintenanceNotification> MaintenanceNotifications => Set<MaintenanceNotification>();

    /// <summary>
    /// Work orders with temporal history
    /// </summary>
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    /// <summary>
    /// Access permission requests with temporal history
    /// </summary>
    public DbSet<AccessPermissionRequest> AccessPermissionRequests => Set<AccessPermissionRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure MaintenanceNotification with temporal table
        modelBuilder.Entity<MaintenanceNotification>(entity =>
        {
            entity.ToTable("MaintenanceNotifications", b => b.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable("MaintenanceNotificationsHistory");
            }));

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.ScheduledDate).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
        });

        // Configure WorkOrder with temporal table
        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.ToTable("WorkOrders", b => b.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable("WorkOrdersHistory");
            }));

            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.DueDate).IsRequired();
            entity.Property(e => e.AssignedTo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
        });

        // Configure AccessPermissionRequest with temporal table
        modelBuilder.Entity<AccessPermissionRequest>(entity =>
        {
            entity.ToTable("AccessPermissionRequests", b => b.IsTemporal(t =>
            {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable("AccessPermissionRequestsHistory");
            }));

            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ResourceName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PermissionType).IsRequired();
            entity.Property(e => e.RequestDate).IsRequired();
            entity.Property(e => e.ApprovedDate).IsRequired(false);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(1000);
        });
    }
}
