using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.ReadRepositories;

public class AssignmentReadRepository(AssignmentContext context) : IAssignmentReadRepository
{
    private readonly AssignmentContext _context = context;

    public async Task<Domain.Aggregates.Assignment?> GetByIdAsync(Guid assignmentId)
    {
        var entity = await _context.Assignments
            .AssignmentQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == assignmentId);
        return entity != null ? Mapping.ConvertEntityToDomainModel(entity) : default;
    }

    public async Task<IReadOnlyCollection<Domain.Aggregates.Assignment>> GetAllByCourseIdAsync(string courseId)
    {
        var entities = await _context.Assignments.AssignmentQuery().AsNoTracking().Where(a => a.CourseId == courseId).ToListAsync();
        return [.. entities.Select(Mapping.ConvertEntityToDomainModel)];
    }

    public async Task<string?> GetCourseIdByAssignmentIdAsync(Guid assignmentId)
    {
        var entity = await _context.Assignments
            .AsNoTracking()
            .Select(a => new { a.Id, a.CourseId })
            .FirstOrDefaultAsync(a => a.Id == assignmentId);
        return entity?.CourseId;
    }
}