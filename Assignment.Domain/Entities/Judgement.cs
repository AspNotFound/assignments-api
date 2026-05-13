using System.ComponentModel.DataAnnotations;
using Assignment.Domain.Exceptions;

namespace Assignment.Domain.Entities;

public class Judgement
{
    internal static Judgement Create(string judgeId, GradingSystemGrade grade, string feedback)
    {
        return new Judgement(judgeId, grade, feedback);
    }

    public static Judgement Hydrate(Guid id, string judgeId, GradingSystemGrade grade, string feedback, DateTimeOffset createdAt, DateTimeOffset modifiedAt)
    {
        return new Judgement(id, judgeId, grade, feedback, createdAt, modifiedAt);
    }

    private Judgement
    (
        string judgeId,
        GradingSystemGrade grade,
        string feedback
    )
    {
        if (!IsValidFeedback(feedback, out var validationErrorMessageFeedback))
        {
            throw new DomainException(validationErrorMessageFeedback);
        }

        Id = Guid.CreateVersion7();
        JudgeId = judgeId;
        Grade = grade;
        Feedback = feedback;
        CreatedAt = DateTimeOffset.UtcNow;
        ModifiedAt = CreatedAt;
    }

    private Judgement
    (
        Guid id,
        string judgeId,
        GradingSystemGrade grade,
        string feedback,
        DateTimeOffset createdAt,
        DateTimeOffset modifiedAt
    )
    {
        Id = id;
        Grade = grade;
        Feedback = feedback;
        JudgeId = judgeId;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; }
    public string JudgeId { get; private set; }
    public GradingSystemGrade Grade { get; private set; }
    public string Feedback { get; private set; }

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset ModifiedAt { get; private set; }

    internal void UpdateJudge(string newJudgeId)
    {
        JudgeId = newJudgeId;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    internal void Regrade(GradingSystemGrade newGrade)
    {
        Grade = newGrade;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    internal void UpdateFeedback(string newFeedback)
    {
        if (!IsValidFeedback(newFeedback, out var validationErrorMessageFeedback))
        {
            throw new DomainException(validationErrorMessageFeedback);
        }
        Feedback = newFeedback;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    private static bool IsValidFeedback(string feedback, out string? validationErrorMessage)
    {
        validationErrorMessage = null;
        return true;
    }
}