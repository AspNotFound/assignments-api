namespace Assignment.Application.Commands.Assignments;

public record CreateAssignmentCommand
(
    Guid GradingSystemId,
    string CourseId,
    string Name,
    string Description,
    DateTimeOffset Deadline
);