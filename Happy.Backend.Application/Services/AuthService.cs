using Happy.Backend.Application.Interfaces;

namespace Happy.Backend.Application.Services;

public interface IAuthService
{
    Task<AuthResult?> AuthenticateAsync(string appSecret, string phone);
}

public class AuthResult
{
    public string Token { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class AuthService : IAuthService
{
    private readonly IAppCredentialRepository _appCredentialRepository;
    private readonly IJwtService _jwtService;

    public AuthService(
        IAppCredentialRepository appCredentialRepository,
        IJwtService jwtService)
    {
        _appCredentialRepository = appCredentialRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResult?> AuthenticateAsync(string appSecret, string phone)
    {
        var app = await _appCredentialRepository.ValidateAsync(appSecret, phone);
        if (app == null) return null;

        var tokenResult = _jwtService.GenerateToken(app);

        return new AuthResult
        {
            Token = tokenResult.Token,
            AppName = app.AppName,
            Phone = app.Phone,
            ExpiresAt = tokenResult.ExpiresAt
        };
    }
}
