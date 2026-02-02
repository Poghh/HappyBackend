using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Models;
using Happy.Backend.Api.Services;
using Happy.Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAppCredentialRepository _appCredentialRepository;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IAppCredentialRepository appCredentialRepository,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings)
    {
        _appCredentialRepository = appCredentialRepository;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AppSecret))
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                CommonMessageConstants.AppSecretRequired));
        }

        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                CommonMessageConstants.PhoneRequired));
        }

        var app = await _appCredentialRepository.ValidateAsync(request.AppSecret.Trim(), request.Phone.Trim());

        if (app == null)
        {
            return Unauthorized(new CommonResponseModel<object>(
                CommonResponseConstants.StatusUnauthorized,
                null,
                CommonMessageConstants.InvalidAppCredentials));
        }

        var token = _jwtService.GenerateToken(app);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var response = new TokenResponse
        {
            Token = token,
            AppName = app.AppName,
            Phone = app.Phone,
            ExpiresAt = expiresAt
        };

        return Ok(new CommonResponseModel<TokenResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }
}
