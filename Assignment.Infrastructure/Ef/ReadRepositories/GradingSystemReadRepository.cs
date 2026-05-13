using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Domain.Aggregates;
using Assignment.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.ReadRepositories;

public class GradingSystemReadRepository(AssignmentContext context) : IGradingSystemReadRepository
{
    private readonly AssignmentContext _context = context;

    public async Task<IReadOnlyCollection<GradingSystem>> GetAllAsync()
    {
        var entities = await _context.GradingSystems.GradingSystemQuery().ToListAsync();
        return [.. entities.Select(Mapping.ConvertEntityToDomainModel)];
    }

    public async Task<GradingSystem?> GetByIdAsync(Guid gradingSystemId)
    {
        var entity = await _context.GradingSystems.GradingSystemQuery().FirstOrDefaultAsync(g => g.Id == gradingSystemId);
        return entity != null ? Mapping.ConvertEntityToDomainModel(entity) : default;
    }
}