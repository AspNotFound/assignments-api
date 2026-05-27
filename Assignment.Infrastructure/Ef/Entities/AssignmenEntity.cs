using Assignment.Infrastructure.Ef.Repositories;

namespace Assignment.Infrastructure.Ef.Entities;

public class AssignmentEntity
{
    public Guid Id { get; set; }
    public Guid GradingSystemId { get; set; }
    public string CourseId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTimeOffset Deadline { get; set; }
    public GradingSystemEntity GradingSystem { get; set; } = null!;
    public ICollection<SubmissionEntity> Submissions { get; set; } = [];
}