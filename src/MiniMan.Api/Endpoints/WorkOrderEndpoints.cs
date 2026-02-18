using FluentValidation;
using MiniMan.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MiniMan.Api.Data;

namespace MiniMan.Api.Endpoints;

/// <summary>
/// Extension methods for WorkOrder endpoints
/// </summary>
public static class WorkOrderEndpoints
{
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

    private static async Task<Ok<IEnumerable<WorkOrder>>> GetAll(MiniManDbContext dbContext)
    {
        var workOrders = await dbContext.WorkOrders.ToListAsync();
        return TypedResults.Ok(workOrders.AsEnumerable());
    }

    private static async Task<Results<Ok<WorkOrder>, NotFound>> GetById(Guid id, MiniManDbContext dbContext)
    {
        var workOrder = await dbContext.WorkOrders.FindAsync(id);
        if (workOrder == null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(workOrder);
    }

    private static async Task<Results<Created<WorkOrder>, BadRequest<string>>> Create(
        WorkOrder workOrder,
        IValidator<WorkOrder> validator,
        MiniManDbContext dbContext)
    {
        var validationResult = await validator.ValidateAsync(workOrder);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        workOrder.Id = Guid.NewGuid();
        workOrder.CreatedDate = DateTime.UtcNow;
        
        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"/api/work-orders/{workOrder.Id}", workOrder);
    }

    private static async Task<Results<Ok<WorkOrder>, BadRequest<string>, NotFound>> Update(
        Guid id,
        WorkOrder workOrder,
        IValidator<WorkOrder> validator,
        MiniManDbContext dbContext)
    {
        var existing = await dbContext.WorkOrders.FindAsync(id);
        if (existing == null)
        {
            return TypedResults.NotFound();
        }

        var validationResult = await validator.ValidateAsync(workOrder);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        existing.OrderNumber = workOrder.OrderNumber;
        existing.Description = workOrder.Description;
        existing.DueDate = workOrder.DueDate;
        existing.AssignedTo = workOrder.AssignedTo;
        existing.Status = workOrder.Status;
        existing.Category = workOrder.Category;

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(existing);
    }

    private static async Task<Results<NoContent, NotFound>> Delete(Guid id, MiniManDbContext dbContext)
    {
        var workOrder = await dbContext.WorkOrders.FindAsync(id);
        if (workOrder == null)
        {
            return TypedResults.NotFound();
        }

        dbContext.WorkOrders.Remove(workOrder);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
