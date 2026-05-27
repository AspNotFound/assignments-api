using Assignment.Api.Extensions;
using Assignment.Application.Commands.Submissions;
using Assignment.Application.Commands.Submissions.Handlers;
using Assignment.Application.Queries.Submissions;
using Assignment.Application.Queries.Submissions.Handlers;

namespace Assignment.Api.EndPoints;

public static class SubmissionEndPoints
{
    public static void MapSubmissionEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/submission").RequireAuthorization();

        group.MapGet("/{id:guid}", Get)
            .WithName("GetSubmissionById")
            .WithDescription("Retrieves a submission by its ID.");

        group.MapPost("/", Post)
            .WithName("CreateSubmission")
            .WithDescription("Creates a new submission.");

        group.MapPut("/", Put)
            .WithName("UpdateSubmission")
            .WithDescription("Updates an existing submission.");

        group.MapDelete("/{id:guid}", Delete)
            .WithName("DeleteSubmission")
            .WithDescription("Deletes a submission by its ID.");

        group.MapPost("/judge", Judge)
            .WithName("JudgeSubmission")
            .WithDescription("Judges a submission by its ID.");
    }

    private static async Task<IResult> Get(Guid id, GetSubmissionByIdHandler handler)
    {
        var result = await handler.HandleAsync(new GetSubmissionByIdQuery(id));
        return result.ToResult();
    }

    private static async Task<IResult> Post(CreateSubmissionCommand command, CreateSubmissionHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> Put(UpdateSubmissionCommand command, UpdateSubmissionHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> Delete(Guid id, DeleteSubmissionHandler handler)
    {
        var command = new DeleteSubmissionCommand(id);
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> Judge(JudgeSubmissionCommand command, JudgeSubmissionHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }
}