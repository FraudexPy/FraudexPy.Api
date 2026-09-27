using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Features.Auth.Commands;
using FraudexPy.Api.Features.Auth.DTOs;
using FraudexPy.Api.Features.Auth.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FraudexPy.Api.Controllers;

public class AuthController : BaseApiController
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await Mediator.Send(new RegisterUserCommand(request));
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await Mediator.Send(new LoginUserCommand(request));
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        if (CurrentUserId == null)
        {
            return Unauthorized(ApiResponse<UserDto>.Fail("Unauthorized access."));
        }

        var result = await Mediator.Send(new GetCurrentUserQuery(CurrentUserId.Value));
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
