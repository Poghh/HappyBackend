namespace Happy.Backend.Domain.Entities;

public class Harvest
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Guid SeasonId { get; set; }
    public string HarvestName { get; set; } = string.Empty;
    public string HarvestDate { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string QuantityUnit { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Buyer { get; set; } = string.Empty;
    public bool RecordDebt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Season? Season { get; set; }
}
