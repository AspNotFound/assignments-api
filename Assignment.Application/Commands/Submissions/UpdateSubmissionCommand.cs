namespace Assignment.Application.Commands.Submissions;

public record UpdateSubmissionCommand
(
    Guid Id,
    string Content,
    IReadOnlyCollection<Attachment> Attachments
);

public record Attachment
(
    Guid Id,
    string Name,
    string FileName,
    string Link
);