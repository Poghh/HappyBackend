using System.Text.Json;
using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Filters;
using Happy.Backend.Api.Models;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("store")]
[Tags("Store")]
public class StoreController : ControllerBase
{
    private readonly IAppCredentialRepository _appCredentialRepository;
    private readonly IStoreFarmerInformationRepository _informationRepository;
    private readonly ISyncRawRepository _syncRawRepository;

    public StoreController(
        IAppCredentialRepository appCredentialRepository,
        IStoreFarmerInformationRepository informationRepository,
        ISyncRawRepository syncRawRepository)
    {
        _appCredentialRepository = appCredentialRepository;
        _informationRepository = informationRepository;
        _syncRawRepository = syncRawRepository;
    }

    [HttpPost("create-information")]
    public async Task<IActionResult> CreateInformation([FromBody] StoreInformationRequest request)
    {
        var validationError = ValidateStoreInformationRequest(request);
        if (validationError != null) return validationError;

        var appCredential = await _appCredentialRepository.GetByPhoneAndAppNameAsync(
            request.Phone.Trim(),
            request.AppName.Trim());

        if (appCredential == null)
        {
            appCredential = await _appCredentialRepository.CreateAsync(
                request.AppName.Trim(),
                request.Phone.Trim());
        }

        var entity = new StoreInformation
        {
            AppCredentialId = appCredential.Id,
            StoreName = request.StoreName.Trim(),
            UserName = request.UserName.Trim()
        };

        await _informationRepository.AddStoreAsync(entity);

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
    [HttpPost("sync")]
    public async Task<IActionResult> Sync([FromBody] SyncRequest request)
    {
        var validationError = ValidateSyncRequest(request);
        if (validationError != null) return validationError;

        var now = DateTime.UtcNow;
        var syncTime = request.SyncTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.SyncTime, DateTimeKind.Utc)
            : request.SyncTime.ToUniversalTime();
        var entity = new SyncRawStore
        {
            Phone = request.Phone.Trim(),
            SyncTime = syncTime,
            SyncData = JsonSerializer.Serialize(request.SyncData),
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _syncRawRepository.AddStoreAsync(entity);
        return Ok(new CommonResponseModel<object>(CommonResponseConstants.StatusSuccess, null, "ok"));
    }

    private static IActionResult? ValidateStoreInformationRequest(StoreInformationRequest request)
    {
        if (request == null)
        {
            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "Invalid JSON"));
        }

        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                CommonMessageConstants.PhoneRequired));
        }

        if (string.IsNullOrWhiteSpace(request.AppName))
        {
            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "AppName is required"));
        }

        if (string.IsNullOrWhiteSpace(request.StoreName))
        {
            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "StoreName is required"));
        }

        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "UserName is required"));
        }

        return null;
    }

    private static IActionResult? ValidateSyncRequest(SyncRequest request)
    {
        if (request == null)
        {
            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "Invalid JSON"));
        }

        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "Phone is required"));
        }

        if (request.SyncTime == default)
        {
            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "SyncTime is required"));
        }

        if (request.SyncData.ValueKind == JsonValueKind.Undefined || request.SyncData.ValueKind == JsonValueKind.Null)
        {
            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "SyncData is required"));
        }

        return null;
    }
}

public class StoreInformationRequest
{
    public string Phone { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class StoreInformationResponse
{
    public int Id { get; set; }
    public int AppCredentialId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
