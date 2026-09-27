namespace FraudexPy.Api.Features.Storage.DTOs;

public record FileUploadResultDto(
    string ObjectName,
    string OriginalFileName,
    string ContentType,
    long FileSizeBytes
);

public record FileDownloadUrlDto(
    string ObjectName,
    string DownloadUrl,
    int ExpirationSeconds
);
