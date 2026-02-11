using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Happy.Backend.Infrastructure.Data;

namespace Happy.Backend.Infrastructure.Repositories;

public class SyncRawRepository : ISyncRawRepository
{
    private readonly HappyDbContext _db;

    public SyncRawRepository(HappyDbContext db)
    {
        _db = db;
    }

    public async Task<SyncRawStore> AddStoreAsync(SyncRawStore entity)
    {
        _db.SyncRawStores.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateStoreAsync(SyncRawStore entity)
    {
        _db.SyncRawStores.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<SyncRawFarmer> AddFarmerAsync(SyncRawFarmer entity)
    {
        _db.SyncRawFarmers.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateFarmerAsync(SyncRawFarmer entity)
    {
        _db.SyncRawFarmers.Update(entity);
        await _db.SaveChangesAsync();
    }
}
