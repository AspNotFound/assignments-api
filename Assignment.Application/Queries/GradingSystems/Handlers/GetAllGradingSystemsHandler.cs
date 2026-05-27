using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;

namespace Assignment.Application.Queries.GradingSystems.Handlers;

public class GetAllGradingSystemsHandler
(
    IGradingSystemReadRepository gradingSystem,
    GradingSystemAuthorizationPolicy authorizationPolicy
)
{
    private readonly IGradingSystemReadRepository _gradingSystem = gradingSystem;
    private readonly GradingSystemAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<IReadOnlyCollection<Dto<Domain.Aggregates.GradingSystem, Permissions>>>> HandleAsync(GetAllGradingSystemsQuery request)
    {
        var canView = _authorizationPolicy.CanAccessGradingSystem();
        if (!canView)
        {
            return Result<IReadOnlyCollection<Dto<Domain.Aggregates.GradingSystem, Permissions>>>.Failure(FailureType.Unauthorized, "User does not have permission to view grading systems.");
        }

        var permissions = new Permissions(Edit: _authorizationPolicy.CanModifyGradingSystem());
        var gradingSystems = await _gradingSystem.GetAllAsync();
        var dtos = gradingSystems.Select(gs => new Dto<Domain.Aggregates.GradingSystem, Permissions>(gs, permissions)).ToList();
        return Result<IReadOnlyCollection<Dto<Domain.Aggregates.GradingSystem, Permissions>>>.Success(dtos);
    }
}
