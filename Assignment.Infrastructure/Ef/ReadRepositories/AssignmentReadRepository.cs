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

    public async Task<IReadOnlyCollection<Domain.Aggregates.Assignment>> GetAll(string? courseId)
    {
        var query = _context.Assignments.AssignmentQuery().AsNoTracking();
        if (!string.IsNullOrEmpty(courseId))
        {
            query = query.Where(a => a.CourseId == courseId);
        }
        var entities = await query.ToListAsync();
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