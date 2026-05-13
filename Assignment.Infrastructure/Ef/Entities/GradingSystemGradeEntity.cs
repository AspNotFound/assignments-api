namespace Assignment.Infrastructure.Ef.Entities;

public class GradingSystemGradeEntity
{
    public Guid Id { get; set; }
    public Guid GradingSystemId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsPassingGrade { get; set; }
    public int Order { get; set; }
    public GradingSystemEntity GradingSystem { get; set; } = null!;
    public ICollection<SubmissionJudgementEntity> SubmissionJudgements { get; set; } = [];
}