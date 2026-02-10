using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Models;
using Happy.Backend.Api.Models.Requests;
using Happy.Backend.Api.Models.Responses;
using Happy.Backend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
    {
        var result = await _authService.AuthenticateAsync(
            request.AppSecret.Trim(),
            request.Phone.Trim());

        if (result == null)
        {
            return Unauthorized(new CommonResponseModel<object>(
                CommonResponseConstants.StatusUnauthorized,
                null,
                CommonMessageConstants.InvalidAppCredentials));
        }

        var response = new TokenResponse
        {
            Token = result.Token,
            AppName = result.AppName,
            Phone = result.Phone,
            ExpiresAt = result.ExpiresAt
        };

        return Ok(new CommonResponseModel<TokenResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }
}
