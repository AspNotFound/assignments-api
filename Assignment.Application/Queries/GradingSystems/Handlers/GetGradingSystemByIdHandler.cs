using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.GradingSystems.Handlers;

public class GetGradingSystemByIdHandler(IGradingSystemReadRepository gradingSystem, GradingSystemAuthorizationPolicy authorizationPolicy)
{
    private readonly IGradingSystemReadRepository _gradingSystem = gradingSystem;
    private readonly GradingSystemAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>> HandleAsync(GetGradingSystemByIdQuery request)
    {
        var canView = _authorizationPolicy.CanAccessGradingSystem();
        if (!canView)
        {
            return Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>.Failure(FailureType.Unauthorized, "User does not have permission to view grading systems.");
        }

        var gradingSystem = await _gradingSystem.GetByIdAsync(request.GradingSystemId);
        if (gradingSystem == null)
        {
            return Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>.Failure(FailureType.NotFound, "Grading system not found.");
        }

        var permissions = new Permissions(Edit: _authorizationPolicy.CanModifyGradingSystem());
        return Result<Dto<Domain.Aggregates.GradingSystem, Permissions>>.Success(new Dto<Domain.Aggregates.GradingSystem, Permissions>(gradingSystem, permissions));
    }
}