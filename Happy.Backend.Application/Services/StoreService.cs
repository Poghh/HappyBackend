using System.Text.Json;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Services;

public interface IStoreService
{
    Task<StoreInformation> CreateAsync(string phone, string appName, string userName, string storeName);
    Task<StoreInformation?> GetByCredentialsAsync(string phone, string appName);
    Task SyncAsync(string phone, DateTime syncTime, JsonElement syncData);
}

public class StoreService : IStoreService
{
    private readonly IAppCredentialRepository _appCredentialRepository;
    private readonly IStoreFarmerInformationRepository _informationRepository;
    private readonly ISyncRawRepository _syncRawRepository;

    public StoreService(
        IAppCredentialRepository appCredentialRepository,
        IStoreFarmerInformationRepository informationRepository,
        ISyncRawRepository syncRawRepository)
    {
        _appCredentialRepository = appCredentialRepository;
        _informationRepository = informationRepository;
        _syncRawRepository = syncRawRepository;
    }

    public async Task<StoreInformation> CreateAsync(string phone, string appName, string userName, string storeName)
    {
        var appCredential = await _appCredentialRepository.GetByPhoneAndAppNameAsync(phone, appName);
        appCredential ??= await _appCredentialRepository.CreateAsync(appName, phone);

        var entity = new StoreInformation
        {
            AppCredentialId = appCredential.Id,
            StoreName = storeName,
            UserName = userName
        };

        await _informationRepository.AddStoreAsync(entity);
        return entity;
    }

    public async Task<StoreInformation?> GetByCredentialsAsync(string phone, string appName)
    {
        var appCredential = await _appCredentialRepository.GetByPhoneAndAppNameAsync(phone, appName);
        if (appCredential == null) return null;

        return await _informationRepository.GetStoreByAppCredentialIdAsync(appCredential.Id);
    }

    public async Task SyncAsync(string phone, DateTime syncTime, JsonElement syncData)
    {
        var now = DateTime.UtcNow;
        var normalizedSyncTime = syncTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(syncTime, DateTimeKind.Utc)
            : syncTime.ToUniversalTime();

        var entity = new SyncRawStore
        {
            Phone = phone,
            SyncTime = normalizedSyncTime,
            SyncData = JsonSerializer.Serialize(syncData),
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _syncRawRepository.AddStoreAsync(entity);
    }
}
