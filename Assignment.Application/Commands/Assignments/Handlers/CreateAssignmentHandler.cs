using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Dtos;
using Assignment.Application.Security.Authorization;
using Assignment.Application.Security.Permissions;
using Assignment.Application.Utility;
using Assignment.Domain.Exceptions;

namespace Assignment.Application.Commands.Assignments.Handlers;

public class CreateAssignmentHandler
(
    IAssignmentRepository assignment,
    IGradingSystemRepository gradingSystem,
    ICourseService courseService,
    AssignmentAuthorizationPolicy authorizationPolicy
)
{
    private readonly IAssignmentRepository _assignment = assignment;
    private readonly IGradingSystemRepository _gradingSystem = gradingSystem;
    private readonly ICourseService _courseService = courseService;
    private readonly AssignmentAuthorizationPolicy _authorizationPolicy = authorizationPolicy;

    public async Task<Result<Dto<Domain.Aggregates.Assignment, Permissions>>> HandleAsync(CreateAssignmentCommand command)
    {
        var courseExists = await _courseService.ExistsAsync(command.CourseId);
        if (!courseExists)
        {
            return Result<Dto<Domain.Aggregates.Assignment, Permissions>>.Failure(FailureType.NotFound, "Course not found.");
        }

        var userIsAllowedToCreateAssignment = await _authorizationPolicy.CanCreateAssignmentForCourseAsync(command.CourseId);
        if (!userIsAllowedToCreateAssignment)
        {
            return Result<Dto<Domain.Aggregates.Assignment, Permissions>>.Failure(FailureType.Unauthorized, "User is not authorized to create assignment.");
        }

        var gradingSystemExists = await _gradingSystem.ExistsAsync(command.GradingSystemId);

        Domain.Aggregates.Assignment assignment;
        try
        {
            assignment = Domain.Aggregates.Assignment.Create(gradingSystemExists, command.GradingSystemId, command.CourseId, command.Name, command.Description, command.Deadline);
        }
        catch (DomainException ex)
        {
            return Result<Dto<Domain.Aggregates.Assignment, Permissions>>.Failure(FailureType.DomainError, ex.Message);
        }

        _assignment.Add(assignment);
        await _assignment.SaveChangesAsync();

        var dto = Dto<Domain.Aggregates.Assignment, Permissions>.Create(assignment, new Permissions(Edit: await _authorizationPolicy.CanModifyAssignmentAsync(assignment.Id)));
        return Result<Dto<Domain.Aggregates.Assignment, Permissions>>.Success(dto);
    }
}