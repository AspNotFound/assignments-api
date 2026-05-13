namespace Assignment.Infrastructure.Ef.Entities;

public class SubmissionAttachmentEntity
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public string Name { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public SubmissionEntity Submission { get; set; } = null!;
}
