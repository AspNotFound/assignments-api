namespace Assignment.Application.Abstractions.Repositories;

public interface IGradingSystemRepository
{
    Task<bool> ExistsAsync(Guid gradingSystemId);
    Task<Domain.Aggregates.GradingSystem?> GetByIdAsync(Guid gradingSystemId);
    Task AddAsync(Domain.Aggregates.GradingSystem gradingSystem);
    Task UpdateAsync(Domain.Aggregates.GradingSystem gradingSystem);
    Task DeleteAsync(Guid gradingSystemId);
}