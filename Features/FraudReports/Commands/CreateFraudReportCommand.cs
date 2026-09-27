using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Domain.Entities;
using FraudexPy.Api.Features.FraudReports.DTOs;
using FraudexPy.Api.Infrastructure.Data;
using MediatR;

namespace FraudexPy.Api.Features.FraudReports.Commands;

public record CreateFraudReportCommand(CreateFraudReportRequestDto Request, Guid UserId) : IRequest<ApiResponse<FraudReportDto>>;

public class CreateFraudReportCommandHandler : IRequestHandler<CreateFraudReportCommand, ApiResponse<FraudReportDto>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CreateFraudReportCommandHandler> _logger;

    public CreateFraudReportCommandHandler(ApplicationDbContext dbContext, ILogger<CreateFraudReportCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ApiResponse<FraudReportDto>> Handle(CreateFraudReportCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        if (string.IsNullOrWhiteSpace(req.Title))
        {
            return ApiResponse<FraudReportDto>.Fail("Title is required.");
        }

        var report = new FraudReport
        {
            Id = Guid.NewGuid(),
            Title = req.Title.Trim(),
            Description = req.Description?.Trim() ?? string.Empty,
            SuspectIdentity = req.SuspectIdentity?.Trim() ?? string.Empty,
            EstimatedLoss = req.EstimatedLoss,
            Currency = string.IsNullOrWhiteSpace(req.Currency) ? "USD" : req.Currency.Trim().ToUpper(),
            Status = "Pending",
            RiskLevel = string.IsNullOrWhiteSpace(req.RiskLevel) ? "Medium" : req.RiskLevel.Trim(),
            IncidentDate = req.IncidentDate ?? DateTime.UtcNow,
            AttachmentObjectName = req.AttachmentObjectName,
            AttachmentFileName = req.AttachmentFileName,
            AttachmentContentType = req.AttachmentContentType,
            ReportedByUserId = command.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.FraudReports.Add(report);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fraud report created with ID {ReportId} by user {UserId}", report.Id, command.UserId);

        var dto = new FraudReportDto(
            report.Id,
            report.Title,
            report.Description,
            report.SuspectIdentity,
            report.EstimatedLoss,
            report.Currency,
            report.Status,
            report.RiskLevel,
            report.AttachmentObjectName,
            report.AttachmentFileName,
            report.AttachmentContentType,
            report.IncidentDate,
            report.CreatedAt,
            report.ReportedByUserId,
            null,
            null
        );

        return ApiResponse<FraudReportDto>.Ok(dto, "Fraud report created successfully.");
    }
}
