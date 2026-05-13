namespace Assignment.Application.Abstractions.Repositories;

public interface IAssignmentRepository
{
    Task<Domain.Aggregates.Assignment?> GetByIdAsync(Guid id);
    Task AddAsync(Domain.Aggregates.Assignment assignment);
    Task UpdateAsync(Domain.Aggregates.Assignment assignment);
    Task DeleteAsync(Guid id);
}