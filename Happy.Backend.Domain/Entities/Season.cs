namespace Happy.Backend.Domain.Entities;

public class Season
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string SeasonName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string FarmingType { get; set; } = string.Empty;
    public double TargetYield { get; set; }
    public string YieldUnit { get; set; } = string.Empty;
    public decimal ExpectedProfit { get; set; }
    public double ScaleValue { get; set; }
    public string ScaleUnit { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<Harvest> Harvests { get; set; } = new List<Harvest>();
}
