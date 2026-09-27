using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Domain.Entities;
using FraudexPy.Api.Features.Auth.DTOs;
using FraudexPy.Api.Infrastructure.Data;
using FraudexPy.Api.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FraudexPy.Api.Features.Auth.Commands;

public record RegisterUserCommand(RegisterRequestDto Request) : IRequest<ApiResponse<AuthResponseDto>>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<AuthResponseDto>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password) || string.IsNullOrWhiteSpace(req.Username))
        {
            return ApiResponse<AuthResponseDto>.Fail("Username, email, and password are required.");
        }

        var normalizedEmail = req.Email.Trim().ToLowerInvariant();
        var normalizedUsername = req.Username.Trim().ToLowerInvariant();

        var existingUser = await _dbContext.Users
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail || u.Username.ToLower() == normalizedUsername, cancellationToken);

        if (existingUser)
        {
            return ApiResponse<AuthResponseDto>.Fail("A user with this email or username already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = req.Username.Trim(),
            Email = req.Email.Trim(),
            FullName = req.FullName?.Trim() ?? req.Username.Trim(),
            PasswordHash = _passwordHasher.HashPassword(req.Password),
            Role = string.IsNullOrWhiteSpace(req.Role) ? "User" : req.Role.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.GenerateToken(user);
        var userDto = new UserDto(user.Id, user.Username, user.Email, user.FullName, user.Role, user.CreatedAt);

        _logger.LogInformation("User {Username} registered successfully with ID {UserId}", user.Username, user.Id);

        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(token, userDto), "Registration successful");
    }
}
