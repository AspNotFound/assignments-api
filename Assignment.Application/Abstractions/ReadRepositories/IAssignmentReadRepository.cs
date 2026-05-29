namespace Assignment.Application.Abstractions.ReadRepositories;

public interface IAssignmentReadRepository
{
    public Task<Domain.Aggregates.Assignment?> GetByIdAsync(Guid assignmentId);
    public Task<IReadOnlyCollection<Domain.Aggregates.Assignment>> GetAll(string? courseId);
    public Task<string?> GetCourseIdByAssignmentIdAsync(Guid assignmentId);

}