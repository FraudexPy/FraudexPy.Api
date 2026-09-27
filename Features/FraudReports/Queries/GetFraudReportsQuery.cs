using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Features.FraudReports.DTOs;
using FraudexPy.Api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FraudexPy.Api.Features.FraudReports.Queries;

public record GetFraudReportsQuery(string? SearchTerm = null, string? Status = null, string? RiskLevel = null) : IRequest<ApiResponse<List<FraudReportDto>>>;

public class GetFraudReportsQueryHandler : IRequestHandler<GetFraudReportsQuery, ApiResponse<List<FraudReportDto>>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetFraudReportsQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<FraudReportDto>>> Handle(GetFraudReportsQuery query, CancellationToken cancellationToken)
    {
        var dbQuery = _dbContext.FraudReports
            .Include(f => f.ReportedByUser)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var search = query.SearchTerm.Trim().ToLower();
            dbQuery = dbQuery.Where(f =>
                f.Title.ToLower().Contains(search) ||
                f.Description.ToLower().Contains(search) ||
                f.SuspectIdentity.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            dbQuery = dbQuery.Where(f => f.Status.ToLower() == query.Status.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(query.RiskLevel))
        {
            dbQuery = dbQuery.Where(f => f.RiskLevel.ToLower() == query.RiskLevel.Trim().ToLower());
        }

        var reports = await dbQuery
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FraudReportDto(
                f.Id,
                f.Title,
                f.Description,
                f.SuspectIdentity,
                f.EstimatedLoss,
                f.Currency,
                f.Status,
                f.RiskLevel,
                f.AttachmentObjectName,
                f.AttachmentFileName,
                f.AttachmentContentType,
                f.IncidentDate,
                f.CreatedAt,
                f.ReportedByUserId,
                f.ReportedByUser != null ? f.ReportedByUser.Username : null,
                f.ReportedByUser != null ? f.ReportedByUser.FullName : null
            ))
            .ToListAsync(cancellationToken);

        return ApiResponse<List<FraudReportDto>>.Ok(reports);
    }
}
