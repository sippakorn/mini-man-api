using FluentValidation;
using MiniMan.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MiniMan.Api.Endpoints;

/// <summary>
/// Extension methods for WorkOrder endpoints
/// </summary>
public static class WorkOrderEndpoints
{
    private static readonly Dictionary<Guid, WorkOrder> _workOrders = new();

    public static void MapWorkOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/work-orders")
            .WithTags("Work Orders");

        group.MapGet("/", GetAll)
            .WithName("GetAllWorkOrders")
            .Produces<IEnumerable<WorkOrder>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetById)
            .WithName("GetWorkOrderById")
            .Produces<WorkOrder>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .WithName("CreateWorkOrder")
            .Produces<WorkOrder>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id}", Update)
            .WithName("UpdateWorkOrder")
            .Produces<WorkOrder>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", Delete)
            .WithName("DeleteWorkOrder")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Ok<IEnumerable<WorkOrder>> GetAll()
    {
        return TypedResults.Ok(_workOrders.Values.AsEnumerable());
    }

    private static Results<Ok<WorkOrder>, NotFound> GetById(Guid id)
    {
        if (_workOrders.TryGetValue(id, out var workOrder))
        {
            return TypedResults.Ok(workOrder);
        }
        return TypedResults.NotFound();
    }

    private static async Task<Results<Created<WorkOrder>, BadRequest<string>>> Create(
        WorkOrder workOrder,
        IValidator<WorkOrder> validator)
    {
        var validationResult = await validator.ValidateAsync(workOrder);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        workOrder.Id = Guid.NewGuid();
        workOrder.CreatedDate = DateTime.UtcNow;
        _workOrders[workOrder.Id] = workOrder;

        return TypedResults.Created($"/api/work-orders/{workOrder.Id}", workOrder);
    }

    private static async Task<Results<Ok<WorkOrder>, BadRequest<string>, NotFound>> Update(
        Guid id,
        WorkOrder workOrder,
        IValidator<WorkOrder> validator)
    {
        if (!_workOrders.ContainsKey(id))
        {
            return TypedResults.NotFound();
        }

        var validationResult = await validator.ValidateAsync(workOrder);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        workOrder.Id = id;
        _workOrders[id] = workOrder;

        return TypedResults.Ok(workOrder);
    }

    private static Results<NoContent, NotFound> Delete(Guid id)
    {
        if (!_workOrders.Remove(id))
        {
            return TypedResults.NotFound();
        }
        return TypedResults.NoContent();
    }
}
