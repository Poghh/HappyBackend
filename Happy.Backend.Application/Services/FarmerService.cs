using System.Text.Json;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Services;

public interface IFarmerService
{
    Task<FarmerInformation> CreateAsync(string phone, string appName, string userName, string farmName);
    Task<FarmerInformation?> GetByCredentialsAsync(string phone, string appName);
    Task SyncAsync(string phone, DateTime syncTime, JsonElement syncData);
}

public class FarmerService : IFarmerService
{
    private readonly IAppCredentialRepository _appCredentialRepository;
    private readonly IStoreFarmerInformationRepository _informationRepository;
    private readonly ISyncRawRepository _syncRawRepository;

    public FarmerService(
        IAppCredentialRepository appCredentialRepository,
        IStoreFarmerInformationRepository informationRepository,
        ISyncRawRepository syncRawRepository)
    {
        _appCredentialRepository = appCredentialRepository;
        _informationRepository = informationRepository;
        _syncRawRepository = syncRawRepository;
    }

    public async Task<FarmerInformation> CreateAsync(string phone, string appName, string userName, string farmName)
    {
        var appCredential = await _appCredentialRepository.GetByPhoneAndAppNameAsync(phone, appName);
        appCredential ??= await _appCredentialRepository.CreateAsync(appName, phone);

        var entity = new FarmerInformation
        {
            AppCredentialId = appCredential.Id,
            UserName = userName,
            FarmName = farmName
        };

        await _informationRepository.AddFarmerAsync(entity);
        return entity;
    }

    public async Task<FarmerInformation?> GetByCredentialsAsync(string phone, string appName)
    {
        var appCredential = await _appCredentialRepository.GetByPhoneAndAppNameAsync(phone, appName);
        if (appCredential == null) return null;

        return await _informationRepository.GetFarmerByAppCredentialIdAsync(appCredential.Id);
    }

    public async Task SyncAsync(string phone, DateTime syncTime, JsonElement syncData)
    {
        var now = DateTime.UtcNow;
        var normalizedSyncTime = syncTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(syncTime, DateTimeKind.Utc)
            : syncTime.ToUniversalTime();

        var entity = new SyncRawFarmer
        {
            Phone = phone,
            SyncTime = normalizedSyncTime,
            SyncData = JsonSerializer.Serialize(syncData),
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _syncRawRepository.AddFarmerAsync(entity);
    }
}
