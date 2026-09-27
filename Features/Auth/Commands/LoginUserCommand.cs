using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Features.Auth.DTOs;
using FraudexPy.Api.Infrastructure.Data;
using FraudexPy.Api.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FraudexPy.Api.Features.Auth.Commands;

public record LoginUserCommand(LoginRequestDto Request) : IRequest<ApiResponse<AuthResponseDto>>;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ApiResponse<AuthResponseDto>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<LoginUserCommandHandler> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        if (string.IsNullOrWhiteSpace(req.EmailOrUsername) || string.IsNullOrWhiteSpace(req.Password))
        {
            return ApiResponse<AuthResponseDto>.Fail("Email/Username and password are required.");
        }

        var identifier = req.EmailOrUsername.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == identifier || u.Username.ToLower() == identifier, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(req.Password, user.PasswordHash))
        {
            return ApiResponse<AuthResponseDto>.Fail("Invalid email/username or password.");
        }

        if (!user.IsActive)
        {
            return ApiResponse<AuthResponseDto>.Fail("Account is deactivated. Please contact support.");
        }

        var token = _jwtTokenService.GenerateToken(user);
        var userDto = new UserDto(user.Id, user.Username, user.Email, user.FullName, user.Role, user.CreatedAt);

        _logger.LogInformation("User {Username} logged in successfully.", user.Username);

        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(token, userDto), "Login successful");
    }
}
