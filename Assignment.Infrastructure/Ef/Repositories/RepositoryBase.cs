using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.Repositories;

public abstract class RepositoryBase<T_DomainModel, T_DomainModelId, T_Entity, T_EntityId, T_DbContext>
where T_DbContext : DbContext
where T_Entity : class
{
    private readonly T_DbContext _dbContext;
    protected DbSet<T_Entity> Set => _dbContext.Set<T_Entity>();

    public RepositoryBase(T_DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(T_DomainModel domainModel)
    {
        var entity = DomainModelToEntity(domainModel);
        await Set.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T_DomainModel domainModel)
    {
        var entity = DomainModelToEntity(domainModel);
        Set.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T_DomainModelId id)
    {
        var entityId = DomainModelIdToEntityId(id);
        var entity = await Set.FindAsync(entityId);
        if (entity != null)
        {
            Set.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    protected abstract T_EntityId DomainModelIdToEntityId(T_DomainModelId domainModelId);
    protected abstract T_Entity DomainModelToEntity(T_DomainModel domainModel);
    protected abstract T_DomainModel EntityToDomainModel(T_Entity entity);
}