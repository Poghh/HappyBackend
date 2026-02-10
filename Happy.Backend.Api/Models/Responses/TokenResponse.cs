namespace Happy.Backend.Api.Models.Responses;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
