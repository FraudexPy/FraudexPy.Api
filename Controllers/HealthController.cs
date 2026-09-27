using FraudexPy.Api.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FraudexPy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Check()
    {
        return Ok(ApiResponse<object>.Ok(new
        {
            Status = "Healthy",
            Service = "FraudexPy.Api",
            Framework = ".NET 10",
            TimestampUtc = DateTime.UtcNow
        }, "FraudexPy API is operational"));
    }
}
