using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Features.Auth.DTOs;
using FraudexPy.Api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FraudexPy.Api.Features.Auth.Queries;

public record GetCurrentUserQuery(Guid UserId) : IRequest<ApiResponse<UserDto>>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ApiResponse<UserDto>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetCurrentUserQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<UserDto>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user == null)
        {
            return ApiResponse<UserDto>.Fail("User not found.");
        }

        var dto = new UserDto(user.Id, user.Username, user.Email, user.FullName, user.Role, user.CreatedAt);
        return ApiResponse<UserDto>.Ok(dto);
    }
}
