using Assignment.Application.Utility;

namespace Assignment.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToResult(this Result result)
    {
        return result.IsSuccess ? Results.Ok() : result.FailureType switch
        {
            FailureType.NotFound => Results.NotFound(result.ErrorMessage),
            FailureType.Unauthorized => Results.Forbid(),
            FailureType.DomainError => Results.BadRequest(result.ErrorMessage),
            _ => Results.BadRequest(result.ErrorMessage)
        };
    }

    public static IResult ToResult<T>(this Result<T> result)
    {
        return result.IsSuccess ? Results.Ok(result.Value) : result.FailureType switch
        {
            FailureType.NotFound => Results.NotFound(result.ErrorMessage),
            FailureType.Unauthorized => Results.Forbid(),
            FailureType.DomainError => Results.BadRequest(result.ErrorMessage),
            _ => Results.BadRequest(result.ErrorMessage)
        };
    }
}