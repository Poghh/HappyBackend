namespace Happy.Backend.Domain.Entities;

public class StockIn
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string? Image { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public string StockInDate { get; set; } = string.Empty;
    public string? OfflineTempCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
