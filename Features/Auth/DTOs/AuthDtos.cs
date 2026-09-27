namespace FraudexPy.Api.Features.Auth.DTOs;

public record RegisterRequestDto(
    string Username,
    string Email,
    string Password,
    string FullName,
    string? Role
);

public record LoginRequestDto(
    string EmailOrUsername,
    string Password
);

public record AuthResponseDto(
    string Token,
    UserDto User
);

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string FullName,
    string Role,
    DateTime CreatedAt
);
