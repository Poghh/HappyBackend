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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppCredentialRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AppName))
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "AppName is required"));
        }

        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                CommonMessageConstants.PhoneRequired));
        }

        var appCredential = await _appCredentialRepository.CreateAsync(
            request.AppName.Trim(),
            request.Phone.Trim());

        var response = new CreateAppCredentialResponse
        {
            Id = appCredential.Id,
            AppSecret = appCredential.AppSecret,
            AppName = appCredential.AppName,
            Phone = appCredential.Phone,
            CreatedAt = appCredential.CreatedAt
        };

        return Ok(new CommonResponseModel<CreateAppCredentialResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _appCredentialRepository.DeactivateAsync(id);

        if (!result)
        {
            return NotFound(new CommonResponseModel<object>(
                CommonResponseConstants.StatusNotFound,
                null,
                "AppCredential not found"));
        }

        return Ok(new CommonResponseModel<object>(
            CommonResponseConstants.StatusSuccess,
            null,
            "AppCredential deactivated"));
    }
}

public class CreateAppCredentialRequest
{
    public string AppName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class CreateAppCredentialResponse
{
    public int Id { get; set; }
    public string AppSecret { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
