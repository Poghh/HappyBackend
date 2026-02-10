using System.ComponentModel.DataAnnotations;
using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Models;
using Happy.Backend.Api.Models.Responses;
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
    public async Task<IActionResult> Exists(
        [FromQuery][Required(ErrorMessage = "Phone is required")] string phone,
        [FromQuery][Required(ErrorMessage = "AppName is required")] string appName)
    {
        var exists = await _appCredentialRepository.ExistsByPhoneAndAppNameAsync(
            phone.Trim(),
            appName.Trim());

        var response = new AppCredentialExistsResponse { Exists = exists };

        return Ok(new CommonResponseModel<AppCredentialExistsResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }

    [HttpGet("secret")]
    public async Task<IActionResult> GetSecret(
        [FromQuery][Required(ErrorMessage = "Phone is required")] string phone,
        [FromQuery][Required(ErrorMessage = "AppName is required")] string appName)
    {
        var appCredential = await _appCredentialRepository.GetByPhoneAndAppNameAsync(
            phone.Trim(),
            appName.Trim());

        if (appCredential == null)
        {
            return NotFound(new CommonResponseModel<object>(
                CommonResponseConstants.StatusNotFound,
                null,
                "AppCredential not found"));
        }

        var response = new AppCredentialSecretResponse { AppSecret = appCredential.AppSecret };

        return Ok(new CommonResponseModel<AppCredentialSecretResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }
}
