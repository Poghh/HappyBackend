using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Api.Services;

public interface IJwtService
{
    string GenerateToken(AppCredential app);
    TokenClaims? ValidateToken(string token);
}

public class TokenClaims
{
    public string AppName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
