using System.ComponentModel.DataAnnotations;
using Assignment.Domain.Entities;
using Assignment.Domain.Exceptions;

namespace Assignment.Domain.Aggregates;

public class Submission
{
    internal static Submission Create(Guid assignmentId, string authorId, string content, IReadOnlyCollection<Attachment> attachments)
    {
        return new Submission(assignmentId, authorId, content, attachments);
    }

    public static Submission Hydrate
    (
        Guid id,
        Guid assignmentId,
        string authorId,
        string content, 
        IReadOnlyCollection<Attachment> attachments,
        DateTimeOffset createdAt,
        DateTimeOffset modifiedAt,
        Judgement? judgement
    )
    {
        return new Submission(id, assignmentId, authorId, content, attachments, createdAt, modifiedAt, judgement);
    }

    private Submission(Guid assignmentId, string authorId, string content, IReadOnlyCollection<Attachment> attachments)
    {
        if (!IsValidContent(content, out var validationErrorMessageContent))
        {
            throw new DomainException(validationErrorMessageContent);
        }

        if (!IsValidAttachments(attachments, out var validationErrorMessageAttachments))
        {
            throw new DomainException(validationErrorMessageAttachments);
        }

        Id = Guid.CreateVersion7();
        AssignmentId = assignmentId;
        AuthorId = authorId;
        Content = content;
        CreatedAt = DateTimeOffset.UtcNow;
        ModifiedAt = DateTimeOffset.UtcNow;
        _attachments = [.. attachments];
    }

    private Submission
    (
        Guid id,
        Guid assignmentId,
        string authorId,
        string content,
        IReadOnlyCollection<Attachment> attachments,
        DateTimeOffset createdAt,
        DateTimeOffset modifiedAt,
        Judgement? judgement
    )
    {
        Id = id;
        AssignmentId = assignmentId;
        AuthorId = authorId;
        Content = content;
        _attachments = [.. attachments];
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Judgement = judgement;
    }

    public Guid Id { get; }
    public Guid AssignmentId { get; }
    public string AuthorId { get; }
    public string Content { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ModifiedAt { get; private set; }
    public Judgement? Judgement { get; private set; }

    private readonly List<Attachment> _attachments;
    public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();

    public bool IsDeletable => Judgement == null;

    internal void Judge(string judgeId, GradingSystemGrade grade, string feedback)
    {
        if (Judgement != null)
        {
            Judgement.UpdateJudge(judgeId);
            Judgement.Regrade(grade);
            Judgement.UpdateFeedback(feedback);
            return;
        }

        Judgement = Judgement.Create(judgeId, grade, feedback);
    }

    internal void EditContent(string newContent)
    {
        if (!IsValidContent(newContent, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        Content = newContent;
    }

    internal void Attach(Attachment attachment)
    {
        if (!IsValidAttachment(_attachments, attachment, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }
        _attachments.Add(attachment);
    }

    internal void Detach(Guid attachmentId)
    {
        var attachmentToRemove = _attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachmentToRemove == null)
        {
            return;
        }
        _attachments.Remove(attachmentToRemove);
    }

    private static bool IsValidContent(string content, out string? validationErrorMessage)
    {
        validationErrorMessage = null;
        return true;
    }

    private static bool IsValidAttachment(List<Attachment> attachments, Attachment attachment, out string? validationErrorMessage)
    {
        if (attachments.Any(a => a.Id == attachment.Id))
        {
            validationErrorMessage = "The same attachment cannot be added more than once.";
            return false;
        }

        if (attachments.Any(a => a.Name == attachment.Name))
        {
            validationErrorMessage = "Attachments must have unique names.";
            return false;
        }

        validationErrorMessage = null;
        return true;
    }

    private static bool IsValidAttachments(IReadOnlyCollection<Attachment> arg0, out string? validationErrorMessage)
    {
        var attachments = new List<Attachment>();
        foreach (var attachment in arg0)
        {
            if (!IsValidAttachment(attachments, attachment, out var validationErrorMessageAttachment))
            {
                validationErrorMessage = validationErrorMessageAttachment;
                return false;
            }
            attachments.Add(attachment);
        }
        validationErrorMessage = null;
        return true;
    }
}