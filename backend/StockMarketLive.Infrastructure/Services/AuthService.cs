using Microsoft.EntityFrameworkCore;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Common;
using StockMarketLive.Domain.Entities;
using StockMarketLive.Infrastructure.Persistence;
using StockMarketLive.Application.DTOs.Auth;
using StockMarketLive.Domain.Constants;

namespace StockMarketLive.Infrastructure.Services;

public class AuthService(AppDbContext context, IPasswordHasher passwordHasher, IJwtProvider jwtProvider) : IAuthService
{
    private readonly AppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

	public async Task<Result<AuthResponse>> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username, ct);
        
        if (user is null || !user.IsActive)
        {
            return Result<AuthResponse>.Failure(AppConstants.ErrorCodes.Auth.InvalidCredentials);
        }

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure(AppConstants.ErrorCodes.Auth.InvalidCredentials);
        }

        var token = _jwtProvider.Generate(user);
        
        return Result<AuthResponse>.Success(new AuthResponse(token, user.Id));
    }

    public async Task<Result<Guid>> RegisterAsync(string username, string email, string password, CancellationToken ct = default)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email, ct))
        {
            return Result<Guid>.Failure(AppConstants.ErrorCodes.Auth.UserAlreadyExists);
        }

        var passwordHash = _passwordHasher.HashPassword(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);

        return Result<Guid>.Success(user.Id);
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .SingleOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
        {
            return Result<UserProfileDto>.Failure(AppConstants.ErrorCodes.Auth.UserNotFound);
        }

        var isAdmin = user.UserRoles.Any(ur => ur.Role.Name == AppConstants.Roles.Admin);
        var roles = user.UserRoles.Select(ur => new RoleDto(ur.Role.Id, ur.Role.Name)).ToList();

        var dto = new UserProfileDto(user.Id, user.Username, user.Email, isAdmin, roles);
        
        return Result<UserProfileDto>.Success(dto);
    }

    public async Task<Result<List<UserProfileDto>>> GetUsersAsync(CancellationToken ct = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync(ct);

        var userDtos = users.Select(u => new UserProfileDto(
            u.Id, 
            u.Username, 
            u.Email, 
            u.UserRoles.Any(ur => ur.Role.Name == AppConstants.Roles.Admin),
			[.. u.UserRoles.Select(ur => new RoleDto(ur.Role.Id, ur.Role.Name))]
		)).ToList();

		return Result<List<UserProfileDto>>.Success(userDtos);
    }
}
