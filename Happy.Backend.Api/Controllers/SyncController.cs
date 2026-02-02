using System.Text.Json;
using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Filters;
using Happy.Backend.Api.Models;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("sync")]
[JwtAuthorize]
public class SyncController : ControllerBase
{
    private readonly ISyncRawRepository _repository;

    public SyncController(ISyncRawRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("store")]
    public async Task<IActionResult> Store([FromBody] SyncRequest request)
    {
        var validationError = ValidateRequest(request);
        if (validationError != null) return validationError;

        var now = DateTime.UtcNow;
        var entity = new SyncRawStore
        {
            Phone = request.Phone.Trim(),
            SyncTime = request.SyncTime,
            SyncData = JsonSerializer.Serialize(request.SyncData),
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _repository.AddStoreAsync(entity);
        return Ok(new CommonResponseModel<object>(CommonResponseConstants.StatusSuccess, null, "ok"));
    }

    [HttpPost("farmer")]
    public async Task<IActionResult> Farmer([FromBody] SyncRequest request)
    {
        var validationError = ValidateRequest(request);
        if (validationError != null) return validationError;

        var now = DateTime.UtcNow;
        var entity = new SyncRawFarmer
        {
            Phone = request.Phone.Trim(),
            SyncTime = request.SyncTime,
            SyncData = JsonSerializer.Serialize(request.SyncData),
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _repository.AddFarmerAsync(entity);
        return Ok(new CommonResponseModel<object>(CommonResponseConstants.StatusSuccess, null, "ok"));
    }

    private IActionResult? ValidateRequest(SyncRequest request)
    {
        if (request == null)
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "Invalid JSON"));
        }

        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "Phone is required"));
        }

        if (request.SyncTime == default)
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "SyncTime is required"));
        }

        if (request.SyncData.ValueKind == JsonValueKind.Undefined || request.SyncData.ValueKind == JsonValueKind.Null)
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "SyncData is required"));
        }

        return null;
    }
}
