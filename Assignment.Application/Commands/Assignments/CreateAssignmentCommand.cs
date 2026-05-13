namespace Assignment.Application.Commands.Assignments;

public record CreateAssignmentCommand
(
    Guid GradingSystemId,
    string CourseId,
    string Title,
    string Description,
    DateTimeOffset DueDate
);