namespace Happy.Backend.Domain.Entities;

public class Activity
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Guid SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public string ActivityName { get; set; } = string.Empty;
    public string DateTime { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string ImageUrls { get; set; } = "[]";
    public int ImageCount { get; set; }
    public System.DateTime CreatedAt { get; set; }
    public System.DateTime UpdatedAt { get; set; }

    public Season? Season { get; set; }
}
