namespace Assignment.Application.Commands.GradingSystem;

public record UpdateGradingSystemCommand
(
    Guid Id,
    string Name,
    List<UpdateGradingSystemGrade> Grades
);

public record UpdateGradingSystemGrade
(
    Guid? Id,
    string Name,
    bool IsPassingGrade,
    int Order
);