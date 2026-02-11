using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Happy.Backend.Infrastructure.Data;

namespace Happy.Backend.Infrastructure.Repositories;

public class HarvestRepository : IHarvestRepository
{
    private readonly HappyDbContext _db;

    public HarvestRepository(HappyDbContext db)
    {
        _db = db;
    }

    public async Task<Harvest?> GetByIdAsync(Guid id)
    {
        return await _db.Harvests.FindAsync(id);
    }

    public async Task AddAsync(Harvest entity)
    {
        _db.Harvests.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Harvest entity)
    {
        _db.Harvests.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.Harvests.FindAsync(id);
        if (entity != null)
        {
            _db.Harvests.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
