using Assignment.Domain.Aggregates;

namespace Assignment.Application.Abstractions.Repositories;

public interface ISubmissionRepository
{
    void Add(Submission submission);
    Task<Submission?> GetByIdAsync(Guid id);
    void Update(Submission submission);
    void Delete(Guid id);
    Task SaveChangesAsync();
}