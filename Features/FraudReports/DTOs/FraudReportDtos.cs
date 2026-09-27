namespace FraudexPy.Api.Features.FraudReports.DTOs;

public record CreateFraudReportRequestDto(
    string Title,
    string Description,
    string SuspectIdentity,
    decimal EstimatedLoss,
    string Currency,
    string RiskLevel,
    DateTime? IncidentDate,
    string? AttachmentObjectName,
    string? AttachmentFileName,
    string? AttachmentContentType
);

public record FraudReportDto(
    Guid Id,
    string Title,
    string Description,
    string SuspectIdentity,
    decimal EstimatedLoss,
    string Currency,
    string Status,
    string RiskLevel,
    string? AttachmentObjectName,
    string? AttachmentFileName,
    string? AttachmentContentType,
    DateTime IncidentDate,
    DateTime CreatedAt,
    Guid ReportedByUserId,
    string? ReporterUsername,
    string? ReporterFullName
);
