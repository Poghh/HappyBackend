namespace Happy.Backend.Api.Models;

public class TokenRequest
{
    public string AppSecret { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
