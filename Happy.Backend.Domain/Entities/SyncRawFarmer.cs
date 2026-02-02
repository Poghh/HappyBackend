namespace Happy.Backend.Domain.Entities;

public class SyncRawFarmer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Phone { get; set; } = string.Empty;
    public DateTime SyncTime { get; set; }
    public string SyncData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
