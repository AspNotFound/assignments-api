namespace Assignment.Application.Commands.Submissions;

public record JudgeSubmissionCommand
(
    Guid SubmissionId,
    Guid GradingSystemGradeId,
    string Feedback
);

public record UpdateJudgementCommand
(
    Guid JudgementId,
    Guid SubmissionId,
    Guid GradingSystemGradeId,
    string Feedback
);