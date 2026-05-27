using Assignment.Api.Extensions;
using Assignment.Application.Commands.Assignments;
using Assignment.Application.Commands.Assignments.Handlers;
using Assignment.Application.Queries.Assignments;
using Assignment.Application.Queries.Assignments.Handlers;

namespace Assignment.Api.EndPoints;

public static class AssignmentEndPoints
{
    public static void MapAssignmentEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/assignment").RequireAuthorization();

        group.MapGet("/", GetAssignments)
            .WithName("GetAssignments")
            .WithDescription("Retrieves all assignments");

        group.MapGet("/{id:guid}", GetAssignmentById)
            .WithName("GetAssignmentById")
            .WithDescription("Retrieves an assignment by its ID.");

        group
            .MapPost("/", PostAssignment)
            .WithName("CreateAssignment")
            .WithDescription("Creates a new assignment.");

        group
            .MapPut("/", PutAssignment)
            .WithName("UpdateAssignment")
            .WithDescription("Updates an existing assignment.");

        group
            .MapDelete("/{id:guid}", DeleteAssignment)
            .WithName("DeleteAssignment")
            .WithDescription("Deletes an existing assignment.");
    }

    private static async Task<IResult> GetAssignmentById(Guid id, GetAssignmentByIdHandler handler)
    {
        var result = await handler.HandleAsync(new GetAssignmentByIdQuery(id));
        return result.ToResult();
    }

    private static async Task<IResult> PostAssignment(CreateAssignmentCommand command, CreateAssignmentHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> PutAssignment(UpdateAssignmentCommand command, UpdateAssignmentHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> DeleteAssignment(Guid id, DeleteAssignmentHandler handler)
    {
        var command = new DeleteAssignmentCommand(id);
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> GetAssignments(string? courseId, GetAllAssignmentsHandler handler)
    {
        var result = await handler.HandleAsync(new GetAllAssignments(courseId));
        return result.ToResult();
    }
}