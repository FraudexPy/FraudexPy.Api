using FraudexPy.Api.Configurations;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace FraudexPy.Api.Infrastructure.Services;

public interface IMinioService
{
    Task EnsureBucketExistsAsync(string? bucketName = null, CancellationToken cancellationToken = default);
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? bucketName = null, CancellationToken cancellationToken = default);
    Task<string> GetPresignedDownloadUrlAsync(string objectName, int expiryInSeconds = 3600, string? bucketName = null);
    Task DeleteFileAsync(string objectName, string? bucketName = null, CancellationToken cancellationToken = default);
}

public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;
    private readonly ILogger<MinioService> _logger;

    public MinioService(IMinioClient minioClient, IOptions<MinioSettings> options, ILogger<MinioService> logger)
    {
        _minioClient = minioClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task EnsureBucketExistsAsync(string? bucketName = null, CancellationToken cancellationToken = default)
    {
        var targetBucket = bucketName ?? _settings.BucketName;
        try
        {
            var existsArgs = new BucketExistsArgs().WithBucket(targetBucket);
            bool found = await _minioClient.BucketExistsAsync(existsArgs, cancellationToken);
            if (!found)
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(targetBucket);
                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
                _logger.LogInformation("MinIO bucket '{Bucket}' created successfully.", targetBucket);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure MinIO bucket '{Bucket}' exists.", targetBucket);
            throw;
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? bucketName = null, CancellationToken cancellationToken = default)
    {
        var targetBucket = bucketName ?? _settings.BucketName;
        await EnsureBucketExistsAsync(targetBucket, cancellationToken);

        var extension = Path.GetExtension(fileName);
        var objectName = $"{Guid.NewGuid()}{extension}";

        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(targetBucket)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
        _logger.LogInformation("Uploaded file '{FileName}' as object '{ObjectName}' in bucket '{Bucket}'.", fileName, objectName, targetBucket);

        return objectName;
    }

    public async Task<string> GetPresignedDownloadUrlAsync(string objectName, int expiryInSeconds = 3600, string? bucketName = null)
    {
        var targetBucket = bucketName ?? _settings.BucketName;
        var presignedArgs = new PresignedGetObjectArgs()
            .WithBucket(targetBucket)
            .WithObject(objectName)
            .WithExpiry(expiryInSeconds);

        return await _minioClient.PresignedGetObjectAsync(presignedArgs);
    }

    public async Task DeleteFileAsync(string objectName, string? bucketName = null, CancellationToken cancellationToken = default)
    {
        var targetBucket = bucketName ?? _settings.BucketName;
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(targetBucket)
            .WithObject(objectName);

        await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
        _logger.LogInformation("Deleted object '{ObjectName}' from bucket '{Bucket}'.", objectName, targetBucket);
    }
}
