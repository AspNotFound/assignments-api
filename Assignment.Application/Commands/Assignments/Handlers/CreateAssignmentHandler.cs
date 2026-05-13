using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;
using Assignment.Domain.Exceptions;

namespace Assignment.Application.Commands.Assignments.Handlers;

public class CreateAssignmentHandler
(
    IAssignmentRepository assignment,
    IUser user,
    IGradingSystemRepository gradingSystem,
    ICourseService courseService
)
{
    private readonly IAssignmentRepository _assignment = assignment;
    private readonly IUser _user = user;
    private readonly IGradingSystemRepository _gradingSystem = gradingSystem;
    private readonly ICourseService _courseService = courseService;

    public async Task<Result<Domain.Aggregates.Assignment>> HandleAsync(CreateAssignmentCommand command)
    {
        var courseExists = await _courseService.ExistsAsync(command.CourseId);
        if (!courseExists)
        {
            return Result<Domain.Aggregates.Assignment>.Failure(FailureType.NotFound, $"Course with id {command.CourseId} does not exist.");
        }

        var userIsAllowedToCreateAssignment = _user.IsAdmin() || (_user.IsTeacher() && await _courseService.IsTeacherOfCourseAsync(_user.UserId, command.CourseId));
        if (!userIsAllowedToCreateAssignment)
        {
            return Result<Domain.Aggregates.Assignment>.Failure(FailureType.Unauthorized, "Only teachers of the course or administrators can create assignments.");
        }

        var gradingSystemExists = await _gradingSystem.ExistsAsync(command.GradingSystemId);

        Domain.Aggregates.Assignment assignment;
        try
        {
            assignment = Domain.Aggregates.Assignment.Create(gradingSystemExists, command.GradingSystemId, command.CourseId, command.Title, command.Description, command.DueDate);
        }
        catch (DomainException ex)
        {
            return Result<Domain.Aggregates.Assignment>.Failure(FailureType.DomainError, ex.Message);
        }

        await _assignment.AddAsync(assignment);
        return Result<Domain.Aggregates.Assignment>.Success(assignment);
    }
}