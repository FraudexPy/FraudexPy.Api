using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Features.Storage.DTOs;
using FraudexPy.Api.Infrastructure.Services;
using MediatR;

namespace FraudexPy.Api.Features.Storage.Queries;

public record GetPresignedDownloadUrlQuery(string ObjectName, int ExpirationSeconds = 3600) : IRequest<ApiResponse<FileDownloadUrlDto>>;

public class GetPresignedDownloadUrlQueryHandler : IRequestHandler<GetPresignedDownloadUrlQuery, ApiResponse<FileDownloadUrlDto>>
{
    private readonly IMinioService _minioService;
    private readonly ILogger<GetPresignedDownloadUrlQueryHandler> _logger;

    public GetPresignedDownloadUrlQueryHandler(IMinioService minioService, ILogger<GetPresignedDownloadUrlQueryHandler> logger)
    {
        _minioService = minioService;
        _logger = logger;
    }

    public async Task<ApiResponse<FileDownloadUrlDto>> Handle(GetPresignedDownloadUrlQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.ObjectName))
        {
            return ApiResponse<FileDownloadUrlDto>.Fail("ObjectName is required.");
        }

        try
        {
            var url = await _minioService.GetPresignedDownloadUrlAsync(query.ObjectName, query.ExpirationSeconds);
            var result = new FileDownloadUrlDto(query.ObjectName, url, query.ExpirationSeconds);

            return ApiResponse<FileDownloadUrlDto>.Ok(result, "Presigned download URL generated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for {ObjectName}", query.ObjectName);
            return ApiResponse<FileDownloadUrlDto>.Fail($"Failed to generate download URL: {ex.Message}");
        }
    }
}
