namespace Assignment.Application.Commands.Assignments;

public record UpdateAssignmentCommand
(
    Guid Id,
    string Name,
    string Description,
    DateTimeOffset Deadline
);