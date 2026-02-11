using System.Text.Json;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Application.Models;
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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IAppCredentialRepository _appCredentialRepository;
    private readonly IStoreFarmerInformationRepository _informationRepository;
    private readonly ISyncRawRepository _syncRawRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IHarvestRepository _harvestRepository;

    public FarmerService(
        IAppCredentialRepository appCredentialRepository,
        IStoreFarmerInformationRepository informationRepository,
        ISyncRawRepository syncRawRepository,
        ISeasonRepository seasonRepository,
        IExpenseRepository expenseRepository,
        IActivityRepository activityRepository,
        IHarvestRepository harvestRepository)
    {
        _appCredentialRepository = appCredentialRepository;
        _informationRepository = informationRepository;
        _syncRawRepository = syncRawRepository;
        _seasonRepository = seasonRepository;
        _expenseRepository = expenseRepository;
        _activityRepository = activityRepository;
        _harvestRepository = harvestRepository;
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

        // 1. Parse syncData trước để biết tổng số items
        var json = syncData.ValueKind == JsonValueKind.String
            ? syncData.GetString()!
            : syncData.GetRawText();
        var rawData = JsonSerializer.Deserialize<FarmerSyncData>(json, JsonOptions);

        var totalItems = (rawData?.Seasons?.Count ?? 0) + (rawData?.Expenses?.Count ?? 0) + (rawData?.Activities?.Count ?? 0) + (rawData?.Harvests?.Count ?? 0);

        // 2. Lưu raw data với status = Processing
        var rawEntity = new SyncRawFarmer
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
        await _syncRawRepository.AddFarmerAsync(rawEntity);

        // 3. Xử lý từng item, theo dõi kết quả
        var processedCount = 0;
        var errors = new List<object>();

        if (rawData?.Seasons != null)
        {
            for (var i = 0; i < rawData.Seasons.Count; i++)
            {
                var season = rawData.Seasons[i];
                try
                {
                    await ProcessSeasonAsync(phone, season, now);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new
                    {
                        index = i,
                        type = "season",
                        queueId = season.QueueId,
                        itemId = season.ItemIdentifier,
                        itemName = season.ItemDescription,
                        syncFlag = season.SyncFlag,
                        error = ex.InnerException?.Message ?? ex.Message
                    });
                }
            }
        }

        if (rawData?.Expenses != null)
        {
            for (var i = 0; i < rawData.Expenses.Count; i++)
            {
                var expense = rawData.Expenses[i];
                try
                {
                    await ProcessExpenseAsync(phone, expense, now);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new
                    {
                        index = i,
                        type = "expense",
                        queueId = expense.QueueId,
                        itemId = expense.ItemIdentifier,
                        itemName = expense.ItemDescription,
                        syncFlag = expense.SyncFlag,
                        error = ex.InnerException?.Message ?? ex.Message
                    });
                }
            }
        }

        if (rawData?.Activities != null)
        {
            for (var i = 0; i < rawData.Activities.Count; i++)
            {
                var activity = rawData.Activities[i];
                try
                {
                    await ProcessActivityAsync(phone, activity, now);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new
                    {
                        index = i,
                        type = "activity",
                        queueId = activity.QueueId,
                        itemId = activity.ItemIdentifier,
                        itemName = activity.ItemDescription,
                        syncFlag = activity.SyncFlag,
                        error = ex.InnerException?.Message ?? ex.Message
                    });
                }
            }
        }

        if (rawData?.Harvests != null)
        {
            for (var i = 0; i < rawData.Harvests.Count; i++)
            {
                var harvest = rawData.Harvests[i];
                try
                {
                    await ProcessHarvestAsync(phone, harvest, now);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new
                    {
                        index = i,
                        type = "harvest",
                        queueId = harvest.QueueId,
                        itemId = harvest.ItemIdentifier,
                        itemName = harvest.ItemDescription,
                        syncFlag = harvest.SyncFlag,
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

        await _syncRawRepository.UpdateFarmerAsync(rawEntity);
    }

    private async Task ProcessSeasonAsync(string phone, SeasonSyncItem item, DateTime now)
    {
        switch (item.SyncFlag?.ToUpperInvariant())
        {
            case "C":
                var newSeason = new Season
                {
                    Id = item.Id,
                    Phone = phone,
                    SeasonName = item.SeasonName ?? string.Empty,
                    Status = item.Status ?? string.Empty,
                    FarmingType = item.FarmingType ?? string.Empty,
                    TargetYield = item.TargetYield ?? 0,
                    YieldUnit = item.YieldUnit ?? string.Empty,
                    ExpectedProfit = item.ExpectedProfit ?? 0,
                    ScaleValue = item.ScaleValue ?? 0,
                    ScaleUnit = item.ScaleUnit ?? string.Empty,
                    StartDate = item.StartDate ?? string.Empty,
                    EndDate = item.EndDate ?? string.Empty,
                    CreatedAt = NormalizeToUtc(item.CreatedAt ?? now),
                    UpdatedAt = now
                };
                await _seasonRepository.AddAsync(newSeason);
                break;

            case "U":
                var existing = await _seasonRepository.GetByIdAsync(item.Id);
                if (existing != null)
                {
                    existing.SeasonName = item.SeasonName ?? existing.SeasonName;
                    existing.Status = item.Status ?? existing.Status;
                    existing.FarmingType = item.FarmingType ?? existing.FarmingType;
                    existing.TargetYield = item.TargetYield ?? existing.TargetYield;
                    existing.YieldUnit = item.YieldUnit ?? existing.YieldUnit;
                    existing.ExpectedProfit = item.ExpectedProfit ?? existing.ExpectedProfit;
                    existing.ScaleValue = item.ScaleValue ?? existing.ScaleValue;
                    existing.ScaleUnit = item.ScaleUnit ?? existing.ScaleUnit;
                    existing.StartDate = item.StartDate ?? existing.StartDate;
                    existing.EndDate = item.EndDate ?? existing.EndDate;
                    existing.UpdatedAt = now;
                    await _seasonRepository.UpdateAsync(existing);
                }
                break;

            case "D":
                await _seasonRepository.DeleteAsync(item.Id);
                break;
        }
    }

    private async Task ProcessExpenseAsync(string phone, ExpenseSyncItem item, DateTime now)
    {
        switch (item.SyncFlag?.ToUpperInvariant())
        {
            case "C":
                var newExpense = new Expense
                {
                    Id = item.Id,
                    Phone = phone,
                    SeasonId = item.SeasonId ?? Guid.Empty,
                    SeasonName = item.SeasonName ?? string.Empty,
                    ExpenName = item.ExpenName ?? string.Empty,
                    Date = item.Date ?? string.Empty,
                    Notes = item.Notes ?? string.Empty,
                    Status = item.Status ?? string.Empty,
                    IsDebt = item.IsDebt ?? false,
                    ExpenPrice = item.ExpenPrice ?? 0,
                    ImageUrls = JsonSerializer.Serialize(item.ImageUrls ?? new List<string>()),
                    ImageCount = item.ImageCount ?? 0,
                    CreatedAt = NormalizeToUtc(item.CreatedAt ?? now),
                    UpdatedAt = now
                };
                await _expenseRepository.AddAsync(newExpense);
                break;

            case "U":
                var existing = await _expenseRepository.GetByIdAsync(item.Id);
                if (existing != null)
                {
                    existing.SeasonId = item.SeasonId ?? existing.SeasonId;
                    existing.SeasonName = item.SeasonName ?? existing.SeasonName;
                    existing.ExpenName = item.ExpenName ?? existing.ExpenName;
                    existing.Date = item.Date ?? existing.Date;
                    existing.Notes = item.Notes ?? existing.Notes;
                    existing.Status = item.Status ?? existing.Status;
                    existing.IsDebt = item.IsDebt ?? existing.IsDebt;
                    existing.ExpenPrice = item.ExpenPrice ?? existing.ExpenPrice;
                    if (item.ImageUrls != null)
                        existing.ImageUrls = JsonSerializer.Serialize(item.ImageUrls);
                    existing.ImageCount = item.ImageCount ?? existing.ImageCount;
                    existing.UpdatedAt = now;
                    await _expenseRepository.UpdateAsync(existing);
                }
                break;

            case "D":
                await _expenseRepository.DeleteAsync(item.Id);
                break;
        }
    }

    private async Task ProcessActivityAsync(string phone, ActivitySyncItem item, DateTime now)
    {
        switch (item.SyncFlag?.ToUpperInvariant())
        {
            case "C":
                var newActivity = new Activity
                {
                    Id = item.Id,
                    Phone = phone,
                    SeasonId = item.SeasonId ?? Guid.Empty,
                    SeasonName = item.SeasonName ?? string.Empty,
                    ActivityName = item.ActivityName ?? string.Empty,
                    DateTime = item.DateTime ?? string.Empty,
                    StatusLabel = item.StatusLabel ?? string.Empty,
                    Notes = item.Notes ?? string.Empty,
                    ImageUrls = JsonSerializer.Serialize(item.ImageUrls ?? new List<string>()),
                    ImageCount = item.ImageCount ?? 0,
                    CreatedAt = NormalizeToUtc(item.CreatedAt ?? now),
                    UpdatedAt = now
                };
                await _activityRepository.AddAsync(newActivity);
                break;

            case "U":
                var existing = await _activityRepository.GetByIdAsync(item.Id);
                if (existing != null)
                {
                    existing.SeasonId = item.SeasonId ?? existing.SeasonId;
                    existing.SeasonName = item.SeasonName ?? existing.SeasonName;
                    existing.ActivityName = item.ActivityName ?? existing.ActivityName;
                    existing.DateTime = item.DateTime ?? existing.DateTime;
                    existing.StatusLabel = item.StatusLabel ?? existing.StatusLabel;
                    existing.Notes = item.Notes ?? existing.Notes;
                    if (item.ImageUrls != null)
                        existing.ImageUrls = JsonSerializer.Serialize(item.ImageUrls);
                    existing.ImageCount = item.ImageCount ?? existing.ImageCount;
                    existing.UpdatedAt = now;
                    await _activityRepository.UpdateAsync(existing);
                }
                break;

            case "D":
                await _activityRepository.DeleteAsync(item.Id);
                break;
        }
    }

    private async Task ProcessHarvestAsync(string phone, HarvestSyncItem item, DateTime now)
    {
        switch (item.SyncFlag?.ToUpperInvariant())
        {
            case "C":
                var newHarvest = new Harvest
                {
                    Id = item.Id,
                    Phone = phone,
                    SeasonId = item.SeasonId ?? Guid.Empty,
                    HarvestName = item.HarvestName ?? string.Empty,
                    HarvestDate = item.HarvestDate ?? string.Empty,
                    Quantity = item.Quantity ?? 0,
                    QuantityUnit = item.QuantityUnit ?? string.Empty,
                    Product = item.Product ?? string.Empty,
                    Price = item.Price ?? 0,
                    Buyer = item.Buyer ?? string.Empty,
                    RecordDebt = item.RecordDebt ?? false,
                    CreatedAt = NormalizeToUtc(item.CreatedAt ?? now),
                    UpdatedAt = now
                };
                await _harvestRepository.AddAsync(newHarvest);
                break;

            case "U":
                var existing = await _harvestRepository.GetByIdAsync(item.Id);
                if (existing != null)
                {
                    existing.SeasonId = item.SeasonId ?? existing.SeasonId;
                    existing.HarvestName = item.HarvestName ?? existing.HarvestName;
                    existing.HarvestDate = item.HarvestDate ?? existing.HarvestDate;
                    existing.Quantity = item.Quantity ?? existing.Quantity;
                    existing.QuantityUnit = item.QuantityUnit ?? existing.QuantityUnit;
                    existing.Product = item.Product ?? existing.Product;
                    existing.Price = item.Price ?? existing.Price;
                    existing.Buyer = item.Buyer ?? existing.Buyer;
                    existing.RecordDebt = item.RecordDebt ?? existing.RecordDebt;
                    existing.UpdatedAt = now;
                    await _harvestRepository.UpdateAsync(existing);
                }
                break;

            case "D":
                await _harvestRepository.DeleteAsync(item.Id);
                break;
        }
    }

    private static DateTime NormalizeToUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(dt, DateTimeKind.Utc)
            : dt.ToUniversalTime();
}
