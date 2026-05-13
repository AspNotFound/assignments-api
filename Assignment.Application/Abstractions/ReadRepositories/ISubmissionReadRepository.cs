namespace Assignment.Application.Abstractions.ReadRepositories;

public interface ISubmissionReadRepository
{
    public Task<Domain.Aggregates.Submission?> GetByIdAsync(Guid submissionId);
    public Task<string?> GetAuthorIdBySubmissionIdAsync(Guid submissionId);
    public Task<IReadOnlyCollection<Domain.Aggregates.Submission>> GetAllByAssignmentIdAsync(Guid assignmentId);
    public Task<Domain.Aggregates.Submission?> GetByAssignmentAndAuthorAsync(Guid assignmentId, string authorId);
    public Task<Guid?> GetAssignmentIdBySubmissionIdAsync(Guid submissionId);
}