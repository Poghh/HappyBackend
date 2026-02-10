using Happy.Backend.Application.Interfaces;
using Happy.Backend.Infrastructure.Settings;
using Minio;
using Minio.DataModel.Args;
using Microsoft.Extensions.Options;

namespace Happy.Backend.Infrastructure.Services;

public class MinioStorageService : IObjectStorageService
{
    private readonly IMinioClient _client;
    private readonly MinioSettings _settings;

    public MinioStorageService(IOptions<MinioSettings> options)
    {
        _settings = options.Value;
        var endpointUri = new Uri(_settings.Endpoint);

        var client = new MinioClient()
            .WithEndpoint(endpointUri.Host, endpointUri.Port)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey);

        if (string.Equals(endpointUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
        {
            client = client.WithSSL();
        }

        _client = client.Build();
    }

    public async Task<string> UploadAsync(
        Stream data,
        string objectName,
        string contentType,
        long size,
        CancellationToken cancellationToken = default)
    {
        var putArgs = new PutObjectArgs()
            .WithBucket(_settings.Bucket)
            .WithObject(objectName)
            .WithStreamData(data)
            .WithObjectSize(size)
            .WithContentType(contentType);

        await _client.PutObjectAsync(putArgs, cancellationToken);

        var envPublicEndpoint = Environment.GetEnvironmentVariable("MINIO_PUBLIC_ENDPOINT");
        var publicBase = string.IsNullOrWhiteSpace(envPublicEndpoint)
            ? (string.IsNullOrWhiteSpace(_settings.PublicEndpoint) ? _settings.Endpoint : _settings.PublicEndpoint)
            : envPublicEndpoint;

        return $"{publicBase.TrimEnd('/')}/{_settings.Bucket}/{objectName}";
    }
}
