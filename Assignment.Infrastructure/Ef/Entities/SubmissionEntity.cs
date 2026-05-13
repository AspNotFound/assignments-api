namespace Assignment.Infrastructure.Ef.Entities;

public class SubmissionEntity
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string AuthorId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
    public Guid? JudgementId { get; set; }
    public AssignmentEntity Assignment { get; set; } = null!;
    public ICollection<SubmissionAttachmentEntity> Attachments { get; set; } = [];
    public SubmissionJudgementEntity? Judgement { get; set; }
}
