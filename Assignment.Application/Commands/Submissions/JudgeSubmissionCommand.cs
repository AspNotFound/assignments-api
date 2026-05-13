namespace Assignment.Application.Commands.Submissions;

public record JudgeSubmissionCommand
(
    Guid SubmissionId,
    Guid GradingSystemGradeId,
    string Feedback
);