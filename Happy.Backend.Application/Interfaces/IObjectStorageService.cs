namespace Happy.Backend.Application.Interfaces;

public interface IObjectStorageService
{
    Task<string> UploadAsync(Stream data, string objectName, string contentType, long size, CancellationToken cancellationToken = default);
}
