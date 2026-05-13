using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Application.Utility;

namespace Assignment.Application.Commands.GradingSystem.Handlers;

public class DeleteGradingSystemHandler
{
    private readonly IGradingSystemRepository _gradingSystem;
    private readonly IUser _user;

    public DeleteGradingSystemHandler(IGradingSystemRepository gradingSystem, IUser user)
    {
        _gradingSystem = gradingSystem;
        _user = user;
    }

    public async Task<Result> HandleAsync(DeleteGradingSystemCommand request)
    {
        var userIsAllowedToDeleteGradingSystem = _user.IsAdmin() || _user.IsTeacher();
        if (!userIsAllowedToDeleteGradingSystem)
        {
            return Result.Failure(FailureType.Unauthorized, "Only admins and teachers can delete grading systems.");
        }

        await _gradingSystem.DeleteAsync(request.GradingSystemId);
        return Result.Success();
    }
}