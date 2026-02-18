using FluentValidation;
using MiniMan.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MiniMan.Api.Data;

namespace MiniMan.Api.Endpoints;

/// <summary>
/// Extension methods for MaintenanceNotification endpoints
/// </summary>
public static class MaintenanceNotificationEndpoints
{
    public static void MapMaintenanceNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/maintenance-notifications")
            .WithTags("Maintenance Notifications");

        group.MapGet("/", GetAll)
            .WithName("GetAllMaintenanceNotifications")
            .Produces<IEnumerable<MaintenanceNotification>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetById)
            .WithName("GetMaintenanceNotificationById")
            .Produces<MaintenanceNotification>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .WithName("CreateMaintenanceNotification")
            .Produces<MaintenanceNotification>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id}", Update)
            .WithName("UpdateMaintenanceNotification")
            .Produces<MaintenanceNotification>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", Delete)
            .WithName("DeleteMaintenanceNotification")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Ok<IEnumerable<MaintenanceNotification>>> GetAll(MiniManDbContext dbContext)
    {
        var notifications = await dbContext.MaintenanceNotifications.ToListAsync();
        return TypedResults.Ok(notifications.AsEnumerable());
    }

    private static async Task<Results<Ok<MaintenanceNotification>, NotFound>> GetById(Guid id, MiniManDbContext dbContext)
    {
        var notification = await dbContext.MaintenanceNotifications.FindAsync(id);
        if (notification == null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(notification);
    }

    private static async Task<Results<Created<MaintenanceNotification>, ValidationProblem>> Create(
        MaintenanceNotification notification,
        IValidator<MaintenanceNotification> validator,
        MiniManDbContext dbContext)
    {
        var validationResult = await validator.ValidateAsync(notification);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(ValidationHelper.ToErrorDictionary(validationResult));
        }

        notification.Id = Guid.NewGuid();
        notification.CreatedDate = DateTime.UtcNow;
        
        dbContext.MaintenanceNotifications.Add(notification);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"/api/maintenance-notifications/{notification.Id}", notification);
    }

    private static async Task<Results<Ok<MaintenanceNotification>, ValidationProblem, NotFound>> Update(
        Guid id,
        MaintenanceNotification notification,
        IValidator<MaintenanceNotification> validator,
        MiniManDbContext dbContext)
    {
        var existing = await dbContext.MaintenanceNotifications.FindAsync(id);
        if (existing == null)
        {
            return TypedResults.NotFound();
        }

        var validationResult = await validator.ValidateAsync(notification);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(ValidationHelper.ToErrorDictionary(validationResult));
        }

        existing.Title = notification.Title;
        existing.Description = notification.Description;
        existing.ScheduledDate = notification.ScheduledDate;
        existing.Status = notification.Status;
        existing.Priority = notification.Priority;

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(existing);
    }

    private static async Task<Results<NoContent, NotFound>> Delete(Guid id, MiniManDbContext dbContext)
    {
        var notification = await dbContext.MaintenanceNotifications.FindAsync(id);
        if (notification == null)
        {
            return TypedResults.NotFound();
        }

        dbContext.MaintenanceNotifications.Remove(notification);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
