using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Happy.Backend.Infrastructure.Data;

namespace Happy.Backend.Infrastructure.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly HappyDbContext _db;

    public ActivityRepository(HappyDbContext db)
    {
        _db = db;
    }

    public async Task<Activity?> GetByIdAsync(Guid id)
    {
        return await _db.Activities.FindAsync(id);
    }

    public async Task AddAsync(Activity entity)
    {
        _db.Activities.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Activity entity)
    {
        _db.Activities.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.Activities.FindAsync(id);
        if (entity != null)
        {
            _db.Activities.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
