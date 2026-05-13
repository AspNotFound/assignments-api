namespace Assignment.Application.Commands.GradingSystem;

public record CreateGradingSystemCommand
(
    string Name,
    string Description,
    List<GradingSystemGrade> Grades
);

public record GradingSystemGrade
(
    string Name,
    bool IsPassingGrade,
    int Order
);