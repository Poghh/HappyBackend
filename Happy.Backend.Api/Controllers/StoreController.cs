using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Filters;
using Happy.Backend.Api.Models;
using Happy.Backend.Api.Models.Requests;
using Happy.Backend.Api.Models.Responses;
using Happy.Backend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("store")]
[Tags("Store")]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpPost("create-information")]
    public async Task<IActionResult> CreateInformation([FromBody] StoreInformationRequest request)
    {
        var entity = await _storeService.CreateAsync(
            request.Phone.Trim(),
            request.AppName.Trim(),
            request.UserName.Trim(),
            request.StoreName.Trim());

        var response = new StoreInformationResponse
        {
            Id = entity.Id,
            AppCredentialId = entity.AppCredentialId,
            StoreName = entity.StoreName,
            UserName = entity.UserName
        };

        return Ok(new CommonResponseModel<StoreInformationResponse>(
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

        var info = await _storeService.GetByCredentialsAsync(phone, appName);
        if (info == null)
        {
            return NotFound(new CommonResponseModel<object>(
                CommonResponseConstants.StatusNotFound,
                null,
                "Store information not found"));
        }

        var response = new StoreInformationResponse
        {
            Id = info.Id,
            AppCredentialId = info.AppCredentialId,
            StoreName = info.StoreName,
            UserName = info.UserName
        };

        return Ok(new CommonResponseModel<StoreInformationResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }

    [JwtAuthorize]
    [HttpPost("sync")]
    public async Task<IActionResult> Sync([FromBody] SyncRequest request)
    {
        await _storeService.SyncAsync(
            request.Phone.Trim(),
            request.SyncTime,
            request.SyncData);

        return Ok(new CommonResponseModel<object>(
            CommonResponseConstants.StatusSuccess,
            null,
            "ok"));
    }
}
