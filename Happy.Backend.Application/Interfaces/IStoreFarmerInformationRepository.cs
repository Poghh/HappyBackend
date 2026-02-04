using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Interfaces;

public interface IStoreFarmerInformationRepository
{
    Task<StoreInformation> AddStoreAsync(StoreInformation entity);
    Task<FarmerInformation> AddFarmerAsync(FarmerInformation entity);
}
