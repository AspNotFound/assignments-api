using Assignment.Api.Extensions;
using Assignment.Application.Commands.GradingSystem;
using Assignment.Application.Commands.GradingSystem.Handlers;
using Assignment.Application.Queries.GradingSystems;
using Assignment.Application.Queries.GradingSystems.Handlers;

namespace Assignment.Api.EndPoints;

public static class GradingSystemEndPoints
{
    public static void MapGradingSystemEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/gradingSystem").RequireAuthorization();

        group.MapGet("/", GetGradingSystems)
        .WithName("GetGradingSystems");

        group.MapGet("/{id:guid}", Get)
        .WithName("GetGradingSystemById");

        group.MapPost("/", Post)
        .WithName("CreateGradingSystem");

        group.MapPut("/", Put)
        .WithName("UpdateGradingSystem");

        group.MapDelete("/{id:guid}", Delete)
        .WithName("DeleteGradingSystem");
    }

    private static async Task<IResult> GetGradingSystems(GetAllGradingSystemsHandler handler)
    {
        var result = await handler.HandleAsync(GetAllGradingSystemsQuery.Instance);
        return result.ToResult();
    }

    private static async Task<IResult> Get(Guid id, GetGradingSystemByIdHandler handler)
    {
        var result = await handler.HandleAsync(new GetGradingSystemByIdQuery(id));
        return result.ToResult();
    }

    private static async Task<IResult> Post(CreateGradingSystemCommand command, CreateGradingSystemHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> Put(UpdateGradingSystemCommand command, UpdateGradingSystemHandler handler)
    {
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }

    private static async Task<IResult> Delete(Guid id, DeleteGradingSystemHandler handler)
    {
        var command = new DeleteGradingSystemCommand(id);
        var result = await handler.HandleAsync(command);
        return result.ToResult();
    }
}