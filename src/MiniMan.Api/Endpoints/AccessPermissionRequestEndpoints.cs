using FluentValidation;
using MiniMan.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MiniMan.Api.Endpoints;

/// <summary>
/// Extension methods for AccessPermissionRequest endpoints
/// </summary>
public static class AccessPermissionRequestEndpoints
{
    private static readonly Dictionary<Guid, AccessPermissionRequest> _requests = new();

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

    private static Ok<IEnumerable<AccessPermissionRequest>> GetAll()
    {
        return TypedResults.Ok(_requests.Values.AsEnumerable());
    }

    private static Results<Ok<AccessPermissionRequest>, NotFound> GetById(Guid id)
    {
        if (_requests.TryGetValue(id, out var request))
        {
            return TypedResults.Ok(request);
        }
        return TypedResults.NotFound();
    }

    private static async Task<Results<Created<AccessPermissionRequest>, BadRequest<string>>> Create(
        AccessPermissionRequest request,
        IValidator<AccessPermissionRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        request.Id = Guid.NewGuid();
        request.RequestDate = DateTime.UtcNow;
        _requests[request.Id] = request;

        return TypedResults.Created($"/api/access-permission-requests/{request.Id}", request);
    }

    private static async Task<Results<Ok<AccessPermissionRequest>, BadRequest<string>, NotFound>> Update(
        Guid id,
        AccessPermissionRequest request,
        IValidator<AccessPermissionRequest> validator)
    {
        if (!_requests.ContainsKey(id))
        {
            return TypedResults.NotFound();
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        request.Id = id;
        _requests[id] = request;

        return TypedResults.Ok(request);
    }

    private static Results<NoContent, NotFound> Delete(Guid id)
    {
        if (!_requests.Remove(id))
        {
            return TypedResults.NotFound();
        }
        return TypedResults.NoContent();
    }
}
