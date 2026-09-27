using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Features.Storage.DTOs;
using FraudexPy.Api.Infrastructure.Services;
using MediatR;

namespace FraudexPy.Api.Features.Storage.Commands;

public record UploadFileCommand(Stream FileStream, string FileName, string ContentType) : IRequest<ApiResponse<FileUploadResultDto>>;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, ApiResponse<FileUploadResultDto>>
{
    private readonly IMinioService _minioService;
    private readonly ILogger<UploadFileCommandHandler> _logger;

    public UploadFileCommandHandler(IMinioService minioService, ILogger<UploadFileCommandHandler> logger)
    {
        _minioService = minioService;
        _logger = logger;
    }

    public async Task<ApiResponse<FileUploadResultDto>> Handle(UploadFileCommand command, CancellationToken cancellationToken)
    {
        if (command.FileStream == null || command.FileStream.Length == 0)
        {
            return ApiResponse<FileUploadResultDto>.Fail("No file stream provided.");
        }

        try
        {
            var objectName = await _minioService.UploadFileAsync(
                command.FileStream,
                command.FileName,
                command.ContentType,
                cancellationToken: cancellationToken
            );

            var result = new FileUploadResultDto(
                objectName,
                command.FileName,
                command.ContentType,
                command.FileStream.Length
            );

            return ApiResponse<FileUploadResultDto>.Ok(result, "File uploaded to MinIO successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to MinIO", command.FileName);
            return ApiResponse<FileUploadResultDto>.Fail($"Failed to upload file to storage: {ex.Message}");
        }
    }
}
