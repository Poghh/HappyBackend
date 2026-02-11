namespace Happy.Backend.Domain.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Guid SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public string ExpenName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsDebt { get; set; }
    public decimal ExpenPrice { get; set; }
    public string ImageUrls { get; set; } = "[]";
    public int ImageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Season? Season { get; set; }
}
