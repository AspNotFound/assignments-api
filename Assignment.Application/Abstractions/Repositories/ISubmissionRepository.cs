using Assignment.Domain.Aggregates;

namespace Assignment.Application.Abstractions.Repositories;

public interface ISubmissionRepository
{
    Task AddAsync(Submission submission);
    Task<Submission?> GetByIdAsync(Guid id);
    Task UpdateAsync(Submission submission);
    Task DeleteAsync(Guid id);

    Task<bool> UserSubmissionExistsAsync(Guid assignmentId, string userId);
}