using Assignment.Application.Abstractions.Repositories;
using Assignment.Domain.Aggregates;
using Assignment.Infrastructure.Ef.Contexts;
using Assignment.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.Repositories;

public class GradingSystemRepository(AssignmentContext context) : RepositoryBase<GradingSystem, Guid, GradingSystemEntity, Guid, AssignmentContext>(context), IGradingSystemRepository
{
    private const string TemporaryGradeNamePrefix = "__tmp_grading_system_grade__";

    public new void Delete(Guid gradingSystemId)
    {
        var trackedEntity = _context.ChangeTracker.Entries<GradingSystemEntity>()
            .FirstOrDefault(e => e.Entity.Id == gradingSystemId);

        if (trackedEntity == null)
        {
            return;
        }

        foreach (var grade in trackedEntity.Entity.Grades.ToList())
        {
            _context.Entry(grade).State = EntityState.Deleted;
        }

        trackedEntity.State = EntityState.Deleted;
    }

    public Task<bool> ExistsAsync(Guid gradingSystemId)
    {
        return Set.AnyAsync(e => e.Id == gradingSystemId);
    }

    public async Task<GradingSystem?> GetByIdAsync(Guid gradingSystemId)
    {
        var entityId = DomainModelIdToEntityId(gradingSystemId);
        var domainModel = await Set.GradingSystemQuery()
            .Where(e => e.Id == entityId)
            .FirstOrDefaultAsync();

        return domainModel != null ? EntityToDomainModel(domainModel) : default;
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

    protected override Guid EntityIdSelector(GradingSystemEntity entity)
    {
        return entity.Id;
    }

    protected override void Apply(GradingSystemEntity target, GradingSystemEntity source)
    {
        target.Apply(source, _context);
    }

    public override async Task SaveChangesAsync()
    {
        var pendingUniqueKeyUpdates = _context.ChangeTracker.Entries<GradingSystemGradeEntity>()
            .Where(entry => entry.State == EntityState.Modified)
            .Where(entry => entry.Property(grade => grade.Order).IsModified || entry.Property(grade => grade.Name).IsModified)
            .Select(entry => new PendingUniqueGradeUpdate(
                entry.Entity.Id,
                entry.Property(grade => grade.Name).CurrentValue,
                entry.Property(grade => grade.Order).CurrentValue))
            .ToList();

        if (pendingUniqueKeyUpdates.Count == 0)
        {
            await base.SaveChangesAsync();
            return;
        }

        for (var index = 0; index < pendingUniqueKeyUpdates.Count; index++)
        {
            var pendingUpdate = pendingUniqueKeyUpdates[index];
            var entry = _context.ChangeTracker.Entries<GradingSystemGradeEntity>()
                .Single(trackedEntry => trackedEntry.Entity.Id == pendingUpdate.GradeId);

            entry.Entity.Name = $"{TemporaryGradeNamePrefix}{pendingUpdate.GradeId:N}";
            entry.Entity.Order = int.MinValue + index;
            entry.Property(grade => grade.Name).IsModified = true;
            entry.Property(grade => grade.Order).IsModified = true;
        }

        await base.SaveChangesAsync();

        foreach (var pendingUpdate in pendingUniqueKeyUpdates)
        {
            var entry = _context.ChangeTracker.Entries<GradingSystemGradeEntity>()
                .Single(trackedEntry => trackedEntry.Entity.Id == pendingUpdate.GradeId);

            entry.Entity.Name = pendingUpdate.Name;
            entry.Entity.Order = pendingUpdate.Order;
            entry.Property(grade => grade.Name).IsModified = true;
            entry.Property(grade => grade.Order).IsModified = true;
        }

        await base.SaveChangesAsync();
    }

    private sealed record PendingUniqueGradeUpdate(Guid GradeId, string Name, int Order);
}