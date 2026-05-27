namespace Assignment.Application.Abstractions.Repositories;

public interface IGradingSystemRepository
{
    Task<bool> ExistsAsync(Guid gradingSystemId);
    Task<Domain.Aggregates.GradingSystem?> GetByIdAsync(Guid gradingSystemId);
    void Add(Domain.Aggregates.GradingSystem gradingSystem);
    void Update(Domain.Aggregates.GradingSystem gradingSystem);
    void Delete(Guid gradingSystemId);
    Task SaveChangesAsync();
}