namespace Assignment.Application.Commands.Submissions;

public record CreateSubmissionCommand
(
    Guid AssignmentId,
    string AuthorId,
    string Content,
    IReadOnlyCollection<CreateSubmissionAttachment> Attachments
);

public record CreateSubmissionAttachment
(
    string Name,
    string FileName,
    string Link
);