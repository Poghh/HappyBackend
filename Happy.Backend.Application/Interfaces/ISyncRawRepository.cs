using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Interfaces;

public interface ISyncRawRepository
{
    Task<SyncRawStore> AddStoreAsync(SyncRawStore entity);
    Task UpdateStoreAsync(SyncRawStore entity);
    Task<SyncRawFarmer> AddFarmerAsync(SyncRawFarmer entity);
    Task UpdateFarmerAsync(SyncRawFarmer entity);
}
