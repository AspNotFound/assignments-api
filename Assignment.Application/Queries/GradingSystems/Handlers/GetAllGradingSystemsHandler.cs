using Assignment.Application.Abstractions.ReadRepositories;

namespace Assignment.Application.Queries.GradingSystems.Handlers;

public class GetAllGradingSystemsHandler
(
    IGradingSystemReadRepository gradingSystem
)
{
    private readonly IGradingSystemReadRepository _gradingSystem = gradingSystem;

    public async Task<IReadOnlyCollection<Domain.Aggregates.GradingSystem>> HandleAsync(GetAllGradingSystemsQuery request)
    {
        return await _gradingSystem.GetAllAsync();
    }
}
