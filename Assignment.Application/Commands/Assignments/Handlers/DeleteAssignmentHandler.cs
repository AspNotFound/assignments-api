using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.Assignments.Handlers;

public class DeleteAssignmentHandler
(
    IAssignmentReadRepository assignmentReadRepository,
    IAssignmentRepository assignmentRepository,
    IUser user,
    ICourseService courseService
)
{
    private readonly IAssignmentReadRepository _assignmentReadRepository = assignmentReadRepository;
    private readonly IAssignmentRepository _assignmentRepository = assignmentRepository;
    private readonly IUser _user = user;
    private readonly ICourseService _courseService = courseService;

    public async Task<Result> HandleAsync(DeleteAssignmentCommand request)
    {
        var assignmentCourseId = await _assignmentReadRepository.GetCourseIdByAssignmentIdAsync(request.AssignmentId);
        if (assignmentCourseId == null)
        {
            return Result.Failure(FailureType.NotFound, $"Assignment with id {request.AssignmentId} not found.");
        }

        var userIsAllowedToDeleteAssignment = _user.IsAdmin() || (_user.IsTeacher() && await _courseService.IsTeacherOfCourseAsync(_user.UserId, assignmentCourseId));
        if (!userIsAllowedToDeleteAssignment)
        {
            return Result.Failure(FailureType.Unauthorized, "Only teachers of the course or administrators can delete assignments.");
        }

        await _assignmentRepository.DeleteAsync(request.AssignmentId);

        return Result.Success();
    }
}