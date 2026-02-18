using FluentValidation;
using MiniMan.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MiniMan.Api.Data;

namespace MiniMan.Api.Endpoints;

/// <summary>
/// Extension methods for AccessPermissionRequest endpoints
/// </summary>
public static class AccessPermissionRequestEndpoints
{
    public static void MapAccessPermissionRequestEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/access-permission-requests")
            .WithTags("Access Permission Requests");

        group.MapGet("/", GetAll)
            .WithName("GetAllAccessPermissionRequests")
            .Produces<IEnumerable<AccessPermissionRequest>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetById)
            .WithName("GetAccessPermissionRequestById")
            .Produces<AccessPermissionRequest>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .WithName("CreateAccessPermissionRequest")
            .Produces<AccessPermissionRequest>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id}", Update)
            .WithName("UpdateAccessPermissionRequest")
            .Produces<AccessPermissionRequest>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", Delete)
            .WithName("DeleteAccessPermissionRequest")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Ok<IEnumerable<AccessPermissionRequest>>> GetAll(MiniManDbContext dbContext)
    {
        var requests = await dbContext.AccessPermissionRequests.ToListAsync();
        return TypedResults.Ok(requests.AsEnumerable());
    }

    private static async Task<Results<Ok<AccessPermissionRequest>, NotFound>> GetById(Guid id, MiniManDbContext dbContext)
    {
        var request = await dbContext.AccessPermissionRequests.FindAsync(id);
        if (request == null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(request);
    }

    private static async Task<Results<Created<AccessPermissionRequest>, ValidationProblem>> Create(
        AccessPermissionRequest request,
        IValidator<AccessPermissionRequest> validator,
        MiniManDbContext dbContext)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            return TypedResults.ValidationProblem(errors);
        }

        request.Id = Guid.NewGuid();
        request.RequestDate = DateTime.UtcNow;
        
        dbContext.AccessPermissionRequests.Add(request);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"/api/access-permission-requests/{request.Id}", request);
    }

    private static async Task<Results<Ok<AccessPermissionRequest>, ValidationProblem, NotFound>> Update(
        Guid id,
        AccessPermissionRequest request,
        IValidator<AccessPermissionRequest> validator,
        MiniManDbContext dbContext)
    {
        var existing = await dbContext.AccessPermissionRequests.FindAsync(id);
        if (existing == null)
        {
            return TypedResults.NotFound();
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            return TypedResults.ValidationProblem(errors);
        }

        existing.RequestedBy = request.RequestedBy;
        existing.ResourceName = request.ResourceName;
        existing.PermissionType = request.PermissionType;
        existing.ApprovedDate = request.ApprovedDate;
        existing.Status = request.Status;
        existing.Reason = request.Reason;

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(existing);
    }

    private static async Task<Results<NoContent, NotFound>> Delete(Guid id, MiniManDbContext dbContext)
    {
        var request = await dbContext.AccessPermissionRequests.FindAsync(id);
        if (request == null)
        {
            return TypedResults.NotFound();
        }

        dbContext.AccessPermissionRequests.Remove(request);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
