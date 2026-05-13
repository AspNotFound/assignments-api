using Assignment.Application.Abstractions.Repositories;
using Assignment.Domain.Aggregates;
using Assignment.Infrastructure.Ef.Contexts;
using Assignment.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.Repositories;

public class SubmissionRepository(AssignmentContext context) : RepositoryBase<Submission, Guid, SubmissionEntity, Guid, AssignmentContext>(context), ISubmissionRepository
{
    public Task<bool> UserSubmissionExistsAsync(Guid assignmentId, string userId)
    {
        return Set.AnyAsync(e => e.AssignmentId == assignmentId && e.AuthorId == userId);
    }

    protected override Guid DomainModelIdToEntityId(Guid domainModelId)
    {
        return domainModelId;
    }

    public async Task<Submission?> GetByIdAsync(Guid id)
    {
        var entityId = DomainModelIdToEntityId(id);
        var entity = await Set.SubmissionQuery().FirstOrDefaultAsync(e => e.Id == entityId);
        return entity != null ? EntityToDomainModel(entity) : default;
    }

    protected override SubmissionEntity DomainModelToEntity(Submission domainModel)
    {
        return Mapping.ConvertDomainModelToEntity(domainModel);
    }

    protected override Submission EntityToDomainModel(SubmissionEntity entity)
    {
        return Mapping.ConvertEntityToDomainModel(entity);
    }
}