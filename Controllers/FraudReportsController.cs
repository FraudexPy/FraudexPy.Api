using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Features.FraudReports.Commands;
using FraudexPy.Api.Features.FraudReports.DTOs;
using FraudexPy.Api.Features.FraudReports.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FraudexPy.Api.Controllers;

[Authorize]
public class FraudReportsController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<FraudReportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports([FromQuery] string? search, [FromQuery] string? status, [FromQuery] string? riskLevel)
    {
        var result = await Mediator.Send(new GetFraudReportsQuery(search, status, riskLevel));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FraudReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FraudReportDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReport([FromBody] CreateFraudReportRequestDto request)
    {
        if (CurrentUserId == null)
        {
            return Unauthorized(ApiResponse<FraudReportDto>.Fail("Unauthorized access."));
        }

        var result = await Mediator.Send(new CreateFraudReportCommand(request, CurrentUserId.Value));
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
