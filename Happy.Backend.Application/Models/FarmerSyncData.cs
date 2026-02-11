using System.Text.Json.Serialization;

namespace Happy.Backend.Application.Models;

public class FarmerSyncData
{
    public List<SeasonSyncItem>? Seasons { get; set; }
    public List<ExpenseSyncItem>? Expenses { get; set; }
    public List<ActivitySyncItem>? Activities { get; set; }
    public List<HarvestSyncItem>? Harvests { get; set; }
}

public class SeasonSyncItem : ISyncItem
{
    public Guid Id { get; set; }
    public string? SeasonName { get; set; }
    public string? Status { get; set; }
    public string? FarmingType { get; set; }
    public double? TargetYield { get; set; }
    public string? YieldUnit { get; set; }
    public decimal? ExpectedProfit { get; set; }
    public double? ScaleValue { get; set; }
    public string? ScaleUnit { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }

    [JsonPropertyName("_createdAt")]
    public DateTime? CreatedAt { get; set; }

    public string? SyncFlag { get; set; }

    [JsonPropertyName("_queueId")]
    public int? QueueId { get; set; }

    public string ItemIdentifier => Id.ToString();
    public string ItemDescription => SeasonName ?? string.Empty;
}

public class ExpenseSyncItem : ISyncItem
{
    public Guid Id { get; set; }
    public string? Date { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
    public string? Season { get; set; }
    public Guid? SeasonId { get; set; }
    public bool? IsDebt { get; set; }
    public string? SeasonName { get; set; }
    public string? ExpenName { get; set; }
    public decimal? ExpenPrice { get; set; }
    public List<string>? ImageUrls { get; set; }
    public int? ImageCount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? SyncFlag { get; set; }

    [JsonPropertyName("_queueId")]
    public int? QueueId { get; set; }

    public string ItemIdentifier => Id.ToString();
    public string ItemDescription => SeasonName ?? string.Empty;
}

public class ActivitySyncItem : ISyncItem
{
    public Guid Id { get; set; }
    public Guid? SeasonId { get; set; }

    public string? SeasonName { get; set; }
    public string? ActivityName { get; set; }
    public string? DateTime { get; set; }
    public string? StatusLabel { get; set; }
    public string? Notes { get; set; }
    public List<string>? ImageUrls { get; set; }
    public int? ImageCount { get; set; }

    public System.DateTime? CreatedAt { get; set; }

    public string? SyncFlag { get; set; }

    [JsonPropertyName("_queueId")]
    public int? QueueId { get; set; }

    public string ItemIdentifier => Id.ToString();
    public string ItemDescription => ActivityName ?? string.Empty;
}

public class HarvestSyncItem : ISyncItem
{
    public Guid Id { get; set; }
    public Guid? SeasonId { get; set; }
    public string? HarvestName { get; set; }
    public string? HarvestDate { get; set; }
    public double? Quantity { get; set; }
    public string? QuantityUnit { get; set; }
    public string? Product { get; set; }
    public decimal? Price { get; set; }
    public string? Buyer { get; set; }
    public bool? RecordDebt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? SyncFlag { get; set; }

    [JsonPropertyName("_queueId")]
    public int? QueueId { get; set; }

    public string ItemIdentifier => Id.ToString();
    public string ItemDescription => HarvestName ?? string.Empty;
}
