using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.Repositories;



public abstract class RepositoryBase<T_DomainModel, T_DomainModelId, T_Entity, T_EntityId, T_DbContext>(T_DbContext dbContext)
where T_DbContext : DbContext
where T_Entity : class
{
    protected readonly T_DbContext _context = dbContext;
    protected DbSet<T_Entity> Set => _context.Set<T_Entity>();

    public void Add(T_DomainModel domainModel)
    {
        var entity = DomainModelToEntity(domainModel);
        Set.Add(entity);
    }

    public void Update(T_DomainModel domainModel)
    {
        var entity = DomainModelToEntity(domainModel);
        var trackedEntity = _context.ChangeTracker.Entries<T_Entity>().FirstOrDefault(e => EntityIdSelector(e.Entity)?.Equals(EntityIdSelector(entity)) == true) ??
            throw new InvalidOperationException($"Entity with id {EntityIdSelector(entity)} is not being tracked.");

        Apply(trackedEntity.Entity, entity);
    }

    public void Delete(T_DomainModelId id)
    {
        var entityId = DomainModelIdToEntityId(id);
        var trackedEntity = _context.ChangeTracker.Entries<T_Entity>().FirstOrDefault(e => EntityIdSelector(e.Entity)?.Equals(entityId) == true);
        if (trackedEntity == null)
        {
            var entity = Set.Find(entityId);
            if (entity != null)
            {
                Set.Remove(entity);
            }
        }
        else
        {
            trackedEntity.State = EntityState.Deleted;
        }
    }

    public virtual async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    protected abstract T_EntityId DomainModelIdToEntityId(T_DomainModelId domainModelId);
    protected abstract T_Entity DomainModelToEntity(T_DomainModel domainModel);
    protected abstract T_DomainModel EntityToDomainModel(T_Entity entity);
    protected abstract T_EntityId EntityIdSelector(T_Entity entity);
    protected abstract void Apply(T_Entity target, T_Entity source);
}