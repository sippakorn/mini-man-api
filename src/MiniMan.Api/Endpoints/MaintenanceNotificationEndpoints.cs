using FluentValidation;
using MiniMan.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MiniMan.Api.Endpoints;

/// <summary>
/// Extension methods for MaintenanceNotification endpoints
/// </summary>
public static class MaintenanceNotificationEndpoints
{
    private static readonly Dictionary<Guid, MaintenanceNotification> _notifications = new();

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

    private static Ok<IEnumerable<MaintenanceNotification>> GetAll()
    {
        return TypedResults.Ok(_notifications.Values.AsEnumerable());
    }

    private static Results<Ok<MaintenanceNotification>, NotFound> GetById(Guid id)
    {
        if (_notifications.TryGetValue(id, out var notification))
        {
            return TypedResults.Ok(notification);
        }
        return TypedResults.NotFound();
    }

    private static async Task<Results<Created<MaintenanceNotification>, BadRequest<string>>> Create(
        MaintenanceNotification notification,
        IValidator<MaintenanceNotification> validator)
    {
        var validationResult = await validator.ValidateAsync(notification);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        notification.Id = Guid.NewGuid();
        notification.CreatedDate = DateTime.UtcNow;
        _notifications[notification.Id] = notification;

        return TypedResults.Created($"/api/maintenance-notifications/{notification.Id}", notification);
    }

    private static async Task<Results<Ok<MaintenanceNotification>, BadRequest<string>, NotFound>> Update(
        Guid id,
        MaintenanceNotification notification,
        IValidator<MaintenanceNotification> validator)
    {
        if (!_notifications.ContainsKey(id))
        {
            return TypedResults.NotFound();
        }

        var validationResult = await validator.ValidateAsync(notification);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        notification.Id = id;
        _notifications[id] = notification;

        return TypedResults.Ok(notification);
    }

    private static Results<NoContent, NotFound> Delete(Guid id)
    {
        if (!_notifications.Remove(id))
        {
            return TypedResults.NotFound();
        }
        return TypedResults.NoContent();
    }
}
