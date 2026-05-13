using Assignment.Application.Abstractions.Repositories;
using Assignment.Domain.Aggregates;
using Assignment.Infrastructure.Ef.Contexts;
using Assignment.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.Repositories;

public class GradingSystemRepository(AssignmentContext context) : RepositoryBase<GradingSystem, Guid, GradingSystemEntity, Guid, AssignmentContext>(context), IGradingSystemRepository
{
    public Task<bool> ExistsAsync(Guid gradingSystemId)
    {
        return Set.AnyAsync(e => e.Id == gradingSystemId);
    }

    public Task<GradingSystem?> GetByIdAsync(Guid gradingSystemId)
    {
        var entityId = DomainModelIdToEntityId(gradingSystemId);
        return Set.GradingSystemQuery()
            .Where(e => e.Id == entityId)
            .Select(e => EntityToDomainModel(e))
            .FirstOrDefaultAsync();
    }

    protected override Guid DomainModelIdToEntityId(Guid domainModelId)
    {
        return domainModelId;
    }

    protected override GradingSystemEntity DomainModelToEntity(GradingSystem domainModel)
    {
        return Mapping.ConvertDomainModelToEntity(domainModel);
    }

    protected override GradingSystem EntityToDomainModel(GradingSystemEntity entity)
    {
        return Mapping.ConvertEntityToDomainModel(entity);
    }
}