namespace Happy.Backend.Domain.Entities;

public class SyncRawStore
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Phone { get; set; } = string.Empty;
    public DateTime SyncTime { get; set; }
    public string SyncData { get; set; } = string.Empty;

    /// <summary>Pending | Processing | Completed | PartiallyCompleted | Failed</summary>
    public string Status { get; set; } = "Pending";

    /// <summary>Total items in this sync batch</summary>
    public int TotalItems { get; set; }

    /// <summary>Number of items processed successfully</summary>
    public int ProcessedItems { get; set; }

    /// <summary>Error details (JSON array) for failed items</summary>
    public string? ErrorDetails { get; set; }

    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
