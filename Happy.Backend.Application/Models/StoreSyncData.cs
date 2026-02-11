using System.Text.Json.Serialization;

namespace Happy.Backend.Application.Models;

/// <summary>
/// Common interface for all sync items — provides shared fields for tracking.
/// </summary>
public interface ISyncItem
{
    string? SyncFlag { get; }
    int? QueueId { get; }
    string ItemIdentifier { get; }
    string ItemDescription { get; }
}

public class StoreSyncData
{
    public List<StockInSyncItem>? StockIns { get; set; }
}

public class StockInSyncItem : ISyncItem
{
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public string? Platform { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public string? Image { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public string? StockInDate { get; set; }

    [JsonPropertyName("_offlineTempCode")]
    public string? OfflineTempCode { get; set; }

    public string? SyncFlag { get; set; }

    [JsonPropertyName("_createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("_queueId")]
    public int? QueueId { get; set; }

    public string ItemIdentifier => ProductCode ?? string.Empty;
    public string ItemDescription => ProductName ?? string.Empty;
}
