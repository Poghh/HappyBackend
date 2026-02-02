namespace Happy.Backend.Domain.Entities;

public class AppCredential
{
    public int Id { get; set; }
    public string AppSecret { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
