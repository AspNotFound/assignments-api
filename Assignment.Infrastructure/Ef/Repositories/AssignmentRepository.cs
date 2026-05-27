using Assignment.Application.Abstractions.Repositories;
using Assignment.Infrastructure.Ef.Contexts;
using Assignment.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.Repositories;

public class AssignmentRepository(AssignmentContext context) : RepositoryBase<Domain.Aggregates.Assignment, Guid, AssignmentEntity, Guid, AssignmentContext>(context), IAssignmentRepository
{
    public async Task<Domain.Aggregates.Assignment?> GetByIdAsync(Guid id)
    {
        var entityId = DomainModelIdToEntityId(id);
        var entity = await Set.AssignmentQuery().FirstOrDefaultAsync(e => e.Id == entityId);
        return entity != null ? EntityToDomainModel(entity) : default;
    }

    protected override Guid DomainModelIdToEntityId(Guid domainModelId)
    {
        return domainModelId;
    }

    protected override AssignmentEntity DomainModelToEntity(Domain.Aggregates.Assignment domainModel)
    {
        return Mapping.ConvertDomainModelToEntity(domainModel);
    }

    protected override Domain.Aggregates.Assignment EntityToDomainModel(AssignmentEntity entity)
    {
        return Mapping.ConvertEntityToDomainModel(entity);
    }

    protected override Guid EntityIdSelector(AssignmentEntity entity)
    {
        return entity.Id;
    }

    protected override void Apply(AssignmentEntity target, AssignmentEntity source)
    {
        target.Apply(source, _context);
    }
}