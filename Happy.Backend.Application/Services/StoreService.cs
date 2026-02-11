using System.Text.Json;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Application.Models;
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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IAppCredentialRepository _appCredentialRepository;
    private readonly IStoreFarmerInformationRepository _informationRepository;
    private readonly ISyncRawRepository _syncRawRepository;
    private readonly IStockInRepository _stockInRepository;

    public StoreService(
        IAppCredentialRepository appCredentialRepository,
        IStoreFarmerInformationRepository informationRepository,
        ISyncRawRepository syncRawRepository,
        IStockInRepository stockInRepository)
    {
        _appCredentialRepository = appCredentialRepository;
        _informationRepository = informationRepository;
        _syncRawRepository = syncRawRepository;
        _stockInRepository = stockInRepository;
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

        // 1. Parse syncData trước để biết tổng số items
        var json = syncData.ValueKind == JsonValueKind.String
            ? syncData.GetString()!
            : syncData.GetRawText();
        var rawData = JsonSerializer.Deserialize<StoreSyncData>(json, JsonOptions);

        var totalItems = (rawData?.StockIns?.Count ?? 0);

        // 2. Lưu raw data với status = Processing
        var rawEntity = new SyncRawStore
        {
            Phone = phone,
            SyncTime = normalizedSyncTime,
            SyncData = JsonSerializer.Serialize(syncData),
            Status = "Processing",
            TotalItems = totalItems,
            ProcessedItems = 0,
            CreatedAt = now,
            UpdatedAt = now,
        };
        await _syncRawRepository.AddStoreAsync(rawEntity);

        // 3. Xử lý từng item, theo dõi kết quả
        var processedCount = 0;
        var errors = new List<object>();

        if (rawData?.StockIns != null)
        {
            for (var i = 0; i < rawData.StockIns.Count; i++)
            {
                var stockIn = rawData.StockIns[i];
                try
                {
                    await ProcessStockInAsync(phone, stockIn, now);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new
                    {
                        index = i,
                        type = "stockIn",
                        queueId = stockIn.QueueId,
                        itemId = stockIn.ItemIdentifier,
                        itemName = stockIn.ItemDescription,
                        syncFlag = stockIn.SyncFlag,
                        error = ex.InnerException?.Message ?? ex.Message
                    });
                }
            }
        }

        // 4. Cập nhật trạng thái raw
        rawEntity.ProcessedItems = processedCount;
        rawEntity.ProcessedAt = DateTime.UtcNow;
        rawEntity.UpdatedAt = DateTime.UtcNow;

        if (errors.Count > 0)
        {
            rawEntity.Status = processedCount > 0 ? "PartiallyCompleted" : "Failed";
            rawEntity.ErrorDetails = JsonSerializer.Serialize(errors);
        }
        else
        {
            rawEntity.Status = "Completed";
        }

        await _syncRawRepository.UpdateStoreAsync(rawEntity);
    }

    private async Task ProcessStockInAsync(string phone, StockInSyncItem item, DateTime now)
    {
        var productCode = item.ProductCode ?? string.Empty;

        switch (item.SyncFlag?.ToUpperInvariant())
        {
            case "C":
                var newStockIn = new StockIn
                {
                    Phone = phone,
                    ProductCode = productCode,
                    ProductName = item.ProductName ?? string.Empty,
                    Category = item.Category ?? string.Empty,
                    Platform = item.Platform ?? string.Empty,
                    Brand = item.Brand ?? string.Empty,
                    Unit = item.Unit ?? string.Empty,
                    Image = item.Image,
                    Quantity = item.Quantity,
                    PurchasePrice = item.PurchasePrice,
                    StockInDate = item.StockInDate ?? string.Empty,
                    OfflineTempCode = item.OfflineTempCode,
                    CreatedAt = NormalizeToUtc(item.CreatedAt ?? now),
                    UpdatedAt = now
                };
                await _stockInRepository.AddAsync(newStockIn);
                break;

            case "U":
                var existing = await _stockInRepository.GetByProductCodeAndPhoneAsync(productCode, phone);
                if (existing != null)
                {
                    existing.ProductName = item.ProductName ?? existing.ProductName;
                    existing.Category = item.Category ?? existing.Category;
                    existing.Platform = item.Platform ?? existing.Platform;
                    existing.Brand = item.Brand ?? existing.Brand;
                    existing.Unit = item.Unit ?? existing.Unit;
                    existing.Image = item.Image ?? existing.Image;
                    existing.Quantity = item.Quantity;
                    existing.PurchasePrice = item.PurchasePrice;
                    existing.StockInDate = item.StockInDate ?? existing.StockInDate;
                    existing.UpdatedAt = now;
                    await _stockInRepository.UpdateAsync(existing);
                }
                break;

            case "D":
                await _stockInRepository.DeleteByProductCodeAndPhoneAsync(productCode, phone);
                break;
        }
    }

    private static DateTime NormalizeToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(dt, DateTimeKind.Utc)
            : dt.ToUniversalTime();
}
