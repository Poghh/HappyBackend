using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Filters;
using Happy.Backend.Api.Models;
using Happy.Backend.Api.Models.Requests;
using Happy.Backend.Api.Models.Responses;
using Happy.Backend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("farmer")]
[Tags("Farmer")]
public class FarmerController : ControllerBase
{
    private readonly IFarmerService _farmerService;

    public FarmerController(IFarmerService farmerService)
    {
        _farmerService = farmerService;
    }

    [HttpPost("create-information")]
    public async Task<IActionResult> CreateInformation([FromBody] FarmerInformationRequest request)
    {
        var entity = await _farmerService.CreateAsync(
            request.Phone.Trim(),
            request.AppName.Trim(),
            request.UserName.Trim(),
            request.FarmName.Trim());

        var response = new FarmerInformationResponse
        {
            Id = entity.Id,
            AppCredentialId = entity.AppCredentialId,
            UserName = entity.UserName,
            FarmName = entity.FarmName
        };

        return Ok(new CommonResponseModel<FarmerInformationResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }

    [JwtAuthorize]
    [HttpGet("get-information")]
    public async Task<IActionResult> GetInformation()
    {
        var appName = HttpContext.Items["AppName"] as string ?? string.Empty;
        var phone = HttpContext.Items["Phone"] as string ?? string.Empty;

        if (string.IsNullOrWhiteSpace(appName) || string.IsNullOrWhiteSpace(phone))
        {
            return Unauthorized(new CommonResponseModel<object>(
                CommonResponseConstants.StatusUnauthorized,
                null,
                CommonMessageConstants.InvalidOrExpiredToken));
        }

        var info = await _farmerService.GetByCredentialsAsync(phone, appName);
        if (info == null)
        {
            return NotFound(new CommonResponseModel<object>(
                CommonResponseConstants.StatusNotFound,
                null,
                "Farmer information not found"));
        }

        var response = new FarmerInformationResponse
        {
            Id = info.Id,
            AppCredentialId = info.AppCredentialId,
            UserName = info.UserName,
            FarmName = info.FarmName
        };

        return Ok(new CommonResponseModel<FarmerInformationResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }

    [JwtAuthorize]
    [HttpPost("sync")]
    public async Task<IActionResult> Sync([FromBody] SyncRequest request)
    {
        await _farmerService.SyncAsync(
            request.Phone.Trim(),
            request.SyncTime,
            request.SyncData);

        return Ok(new CommonResponseModel<object>(
            CommonResponseConstants.StatusSuccess,
            null,
            "ok"));
    }
}
