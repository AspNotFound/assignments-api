using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.GradingSystem.Handlers;

public class DeleteGradingSystemHandler(IGradingSystemRepository gradingSystem, GradingSystemAuthorizationPolicy authorizationPolicy)
{
    private readonly IGradingSystemRepository _gradingSystem = gradingSystem;
    private readonly GradingSystemAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result> HandleAsync(DeleteGradingSystemCommand request)
    {
        var userIsAllowedToModifyGradingSystem = _authorizationPolicy.CanModifyGradingSystem();
        if (!userIsAllowedToModifyGradingSystem)
        {
            return Result.Failure(FailureType.Unauthorized, "User is not authorized to delete grading systems.");
        }

        _gradingSystem.Delete(request.GradingSystemId);
        await _gradingSystem.SaveChangesAsync();
        return Result.Success();
    }
}