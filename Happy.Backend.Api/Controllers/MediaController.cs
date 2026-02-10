using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Filters;
using Happy.Backend.Api.Models;
using Happy.Backend.Api.Models.Responses;
using Happy.Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("media")]
[Tags("Media")]
public class MediaController : ControllerBase
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/heic",
        "image/heif"
    ];

    private readonly IObjectStorageService _storageService;

    public MediaController(IObjectStorageService storageService)
    {
        _storageService = storageService;
    }

    [JwtAuthorize]
    [HttpPost("upload")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "File is required"));
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            return BadRequest(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                "Unsupported file type"));
        }

        var extension = Path.GetExtension(Path.GetFileName(file.FileName));
        var objectName = $"uploads/{DateTime.UtcNow:yyyyMMdd}/{Guid.NewGuid():N}{extension}";

        await using var stream = file.OpenReadStream();
        var url = await _storageService.UploadAsync(
            stream,
            objectName,
            file.ContentType,
            file.Length,
            cancellationToken);

        var response = new UploadResponse
        {
            ObjectKey = objectName,
            Url = url
        };

        return Ok(new CommonResponseModel<UploadResponse>(
            CommonResponseConstants.StatusSuccess,
            response,
            "ok"));
    }
}
