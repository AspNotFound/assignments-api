namespace Assignment.Application.Abstractions.ReadRepositories;

public interface IGradingSystemReadRepository
{
    public Task<IReadOnlyCollection<Domain.Aggregates.GradingSystem>> GetAllAsync();
    public Task<Domain.Aggregates.GradingSystem?> GetByIdAsync(Guid gradingSystemId);
}