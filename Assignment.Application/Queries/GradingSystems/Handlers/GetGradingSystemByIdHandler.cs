using Assignment.Application.Abstractions.ReadRepositories;

namespace Assignment.Application.Queries.GradingSystems.Handlers;

public class GetGradingSystemByIdHandler(IGradingSystemReadRepository gradingSystem)
{
    private readonly IGradingSystemReadRepository _gradingSystem = gradingSystem;

    public async Task<Domain.Aggregates.GradingSystem?> HandleAsync(GetGradingSystemByIdQuery request)
    {
        return await _gradingSystem.GetByIdAsync(request.GradingSystemId);
    }
}