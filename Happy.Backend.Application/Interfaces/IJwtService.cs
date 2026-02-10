using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Interfaces;

public interface IJwtService
{
    JwtTokenResult GenerateToken(AppCredential app);
    TokenClaims? ValidateToken(string token);
}

public class JwtTokenResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class TokenClaims
{
    public string AppName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
