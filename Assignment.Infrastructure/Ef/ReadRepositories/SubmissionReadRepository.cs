using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Domain.Aggregates;
using Assignment.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.ReadRepositories;

public class SubmissionReadRepository(AssignmentContext context) : ISubmissionReadRepository
{
    private readonly AssignmentContext _context = context;

    public async Task<IReadOnlyCollection<Submission>> GetAllByAssignmentIdAsync(Guid assignmentId)
    {
        var entities = await _context
            .Submissions
            .SubmissionQuery()
            .Where(s => s.AssignmentId == assignmentId)
            .ToListAsync();

        return entities.Select(e => Mapping.ConvertEntityToDomainModel(e)).ToList();
    }

    public async Task<Guid?> GetAssignmentIdBySubmissionIdAsync(Guid submissionId)
    {
        var entity = await _context
            .Submissions
            .AsNoTracking()
            .Select(s => new { s.Id, s.AssignmentId })
            .FirstOrDefaultAsync(s => s.Id == submissionId);
        return entity?.AssignmentId;
    }

    public async Task<string?> GetAuthorIdBySubmissionIdAsync(Guid submissionId)
    {
        var entity = await _context
            .Submissions
            .AsNoTracking()
            .Select(s => new { s.Id, s.AuthorId })
            .FirstOrDefaultAsync(s => s.Id == submissionId);
        return entity?.AuthorId;
    }

    public async Task<Submission?> GetByAssignmentAndAuthorAsync(Guid assignmentId, string authorId)
    {
        var entity = await _context
            .Submissions
            .SubmissionQuery()
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.AuthorId == authorId);

        return entity != null ? Mapping.ConvertEntityToDomainModel(entity) : default;
    }

    public async Task<Submission?> GetByIdAsync(Guid submissionId)
    {
        var entity = await _context
            .Submissions
            .SubmissionQuery()
            .FirstOrDefaultAsync(s => s.Id == submissionId);

        return entity != null ? Mapping.ConvertEntityToDomainModel(entity) : default;
    }

    public async Task<bool> UserSubmissionExistsAsync(Guid assignmentId, string userId)
    {
        var exists = await _context.Submissions.AnyAsync(s => s.AssignmentId == assignmentId && s.AuthorId == userId);
        return exists;
    }
}