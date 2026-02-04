using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Models;
using Happy.Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("app-credentials")]
public class AppCredentialController : ControllerBase
{
    private readonly IAppCredentialRepository _appCredentialRepository;

    public AppCredentialController(IAppCredentialRepository appCredentialRepository)
    {
        _appCredentialRepository = appCredentialRepository;
    }

    [HttpGet("exists")]
    public async Task<IActionResult> Exists([FromQuery] string phone, [FromQuery] string appName)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                CommonMessageConstants.PhoneRequired));
        }

        if (string.IsNullOrWhiteSpace(appName))
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "AppName is required"));
        }

        var exists = await _appCredentialRepository.ExistsByPhoneAndAppNameAsync(
            phone.Trim(),
            appName.Trim());

        var response = new AppCredentialExistsResponse
        {
            Exists = exists
        };

        return Ok(new CommonResponseModel<AppCredentialExistsResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }

    [HttpGet("secret")]
    public async Task<IActionResult> GetSecret([FromQuery] string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                CommonMessageConstants.PhoneRequired));
        }

        var appCredential = await _appCredentialRepository.GetLatestByPhoneAsync(phone.Trim());

        if (appCredential == null)
        {
            return NotFound(new CommonResponseModel<object>(
                CommonResponseConstants.StatusNotFound,
                null,
                "AppCredential not found"));
        }

        var response = new AppCredentialSecretResponse
        {
            AppSecret = appCredential.AppSecret
        };

        return Ok(new CommonResponseModel<AppCredentialSecretResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }
}

public class AppCredentialExistsResponse
{
    public bool Exists { get; set; }
}

public class AppCredentialSecretResponse
{
    public string AppSecret { get; set; } = string.Empty;
}
