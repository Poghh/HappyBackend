using System.Text.Json;
using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Filters;
using Happy.Backend.Api.Models;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("farmer")]
[Tags("Farmer")]
public class FarmerController : ControllerBase
{
    private readonly IAppCredentialRepository _appCredentialRepository;
    private readonly IStoreFarmerInformationRepository _informationRepository;
    private readonly ISyncRawRepository _syncRawRepository;

    public FarmerController(
        IAppCredentialRepository appCredentialRepository,
        IStoreFarmerInformationRepository informationRepository,
        ISyncRawRepository syncRawRepository)
    {
        _appCredentialRepository = appCredentialRepository;
        _informationRepository = informationRepository;
        _syncRawRepository = syncRawRepository;
    }

    [HttpPost("create-information")]
    public async Task<IActionResult> CreateInformation([FromBody] FarmerInformationRequest request)
    {
        var validationError = ValidateFarmerInformationRequest(request);
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

        var entity = new FarmerInformation
        {
            AppCredentialId = appCredential.Id,
            UserName = request.UserName.Trim()
        };

        await _informationRepository.AddFarmerAsync(entity);

        var response = new FarmerInformationResponse
        {
            Id = entity.Id,
            AppCredentialId = entity.AppCredentialId,
            UserName = entity.UserName
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
        var validationError = ValidateSyncRequest(request);
        if (validationError != null) return validationError;

        var now = DateTime.UtcNow;
        var syncTime = request.SyncTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.SyncTime, DateTimeKind.Utc)
            : request.SyncTime.ToUniversalTime();
        var entity = new SyncRawFarmer
        {
            Phone = request.Phone.Trim(),
            SyncTime = syncTime,
            SyncData = JsonSerializer.Serialize(request.SyncData),
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _syncRawRepository.AddFarmerAsync(entity);
        return Ok(new CommonResponseModel<object>(CommonResponseConstants.StatusSuccess, null, "ok"));
    }

    private static IActionResult? ValidateFarmerInformationRequest(FarmerInformationRequest request)
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

public class FarmerInformationRequest
{
    public string Phone { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class FarmerInformationResponse
{
    public int Id { get; set; }
    public int AppCredentialId { get; set; }
    public string UserName { get; set; } = string.Empty;
}
