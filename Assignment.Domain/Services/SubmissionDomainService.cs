using Assignment.Domain.Aggregates;
using Assignment.Domain.Entities;
using Assignment.Domain.Exceptions;

namespace Assignment.Domain.Services;

public class SubmissionDomainService
{
    public SubmissionDomainService()
    {

    }

    public static void Update
    (
        Aggregates.Assignment assignment,
        Submission submission,
        string newContent,
        IReadOnlyCollection<Attachment> newAttachments
    )
    {
        if (submission.AssignmentId != assignment.Id)
        {
            throw new DomainException("The submission does not belong to the specified assignment.");
        }

        if (!assignment.IsAllowedToEditSubmissions())
        {
            throw new DomainException("Submissions for this assignment cannot be edited.");
        }

        submission.EditContent(newContent);

        var deletedAttachments = submission.Attachments.Where(a => !newAttachments.Any(na => na.Id == a.Id)).ToList();
        var addedAttachments = newAttachments.Where(na => !submission.Attachments.Any(a => a.Id == na.Id)).ToList();
        var updatedAttachments = newAttachments.Where(na => submission.Attachments.Any(a => a.Id == na.Id)).ToList();

        foreach (var deletedAttachment in deletedAttachments)
        {
            submission.Detach(deletedAttachment.Id);
        }

        foreach (var addedAttachment in addedAttachments)
        {
            submission.Attach(addedAttachment);
        }

        foreach (var updatedAttachment in updatedAttachments)
        {
            var existingAttachment = submission.Attachments.First(a => a.Id == updatedAttachment.Id);
            if (existingAttachment.Name != updatedAttachment.Name)
            {
                existingAttachment.Rename(updatedAttachment.Name);
            }
        }
    }

    public static void Judge
    (
        Aggregates.Assignment assignment,
        GradingSystem gradingSystem,
        Submission submission,
        GradingSystemGrade grade,
        string judgeId,
        string feedback
    )
    {
        if (gradingSystem.Id != assignment.GradingSystemId)
        {
            throw new DomainException("The grading system does not belong to the specified assignment.");
        }

        if (assignment.Id != submission.AssignmentId)
        {
            throw new DomainException("The submission does not belong to the specified assignment.");
        }

        if (gradingSystem.Grades.All(g => g.Id != grade.Id))
        {
            throw new DomainException("The specified grade does not belong to the grading system of the assignment.");
        }

        submission.Judge(judgeId, grade, feedback);
    }
}