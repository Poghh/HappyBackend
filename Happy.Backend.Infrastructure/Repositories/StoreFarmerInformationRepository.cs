using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Happy.Backend.Infrastructure.Data;

namespace Happy.Backend.Infrastructure.Repositories;

public class StoreFarmerInformationRepository : IStoreFarmerInformationRepository
{
    private readonly HappyDbContext _db;

    public StoreFarmerInformationRepository(HappyDbContext db)
    {
        _db = db;
    }

    public async Task<StoreInformation> AddStoreAsync(StoreInformation entity)
    {
        _db.StoreInformation.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<FarmerInformation> AddFarmerAsync(FarmerInformation entity)
    {
        _db.FarmerInformation.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}
