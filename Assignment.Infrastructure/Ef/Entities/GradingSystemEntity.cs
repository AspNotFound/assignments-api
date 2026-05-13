namespace Assignment.Infrastructure.Ef.Entities;

public class GradingSystemEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<AssignmentEntity> Assignments { get; set; } = [];
    public ICollection<GradingSystemGradeEntity> Grades { get; set; } = [];
}
