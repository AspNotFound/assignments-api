namespace Assignment.Application.Abstractions.Repositories;

public interface IAssignmentRepository
{
    Task<Domain.Aggregates.Assignment?> GetByIdAsync(Guid id);
    void Add(Domain.Aggregates.Assignment assignment);
    void Update(Domain.Aggregates.Assignment assignment);
    void Delete(Guid id);
    Task SaveChangesAsync();
}