using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Happy.Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Happy.Backend.Infrastructure.Repositories;

public class SeasonRepository : ISeasonRepository
{
    private readonly HappyDbContext _db;

    public SeasonRepository(HappyDbContext db)
    {
        _db = db;
    }

    public async Task<Season?> GetByIdAsync(Guid id)
    {
        return await _db.Seasons.FindAsync(id);
    }

    public async Task AddAsync(Season entity)
    {
        _db.Seasons.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Season entity)
    {
        _db.Seasons.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.Seasons.FindAsync(id);
        if (entity != null)
        {
            _db.Seasons.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
