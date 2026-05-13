namespace Assignment.Infrastructure.Ef.Entities;

public class SubmissionJudgementEntity
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid GradingSystemGradeId { get; set; }
    public string JudgeId { get; set; } = null!;
    public string Feedback { get; set; } = null!;
    public SubmissionEntity Submission { get; set; } = null!;
    public GradingSystemGradeEntity GradingSystemGrade { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
}