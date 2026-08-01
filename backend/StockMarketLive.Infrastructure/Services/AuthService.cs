using Microsoft.EntityFrameworkCore;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Common;
using StockMarketLive.Domain.Entities;
using StockMarketLive.Infrastructure.Persistence;
using StockMarketLive.Application.DTOs.Auth;

namespace StockMarketLive.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(AppDbContext context, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username, cancellationToken);
        
        if (user is null || !user.IsActive)
        {
            return Result<AuthResponse>.Failure("Geçersiz kullanıcı adı veya şifre.");
        }

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure("Geçersiz kullanıcı adı veya şifre.");
        }

        var token = _jwtProvider.Generate(user);
        
        return Result<AuthResponse>.Success(new AuthResponse(token, user.Id));
    }

    public async Task<Result<Guid>> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email, cancellationToken))
        {
            return Result<Guid>.Failure("Bu kullanıcı adı veya e-posta zaten kullanımda.");
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
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id);
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Result<UserProfileDto>.Failure("Kullanıcı bulunamadı.");
        }

        var isAdmin = user.UserRoles.Any(ur => ur.Role.Name == "Admin");

        var dto = new UserProfileDto(user.Id, user.Username, user.Email, isAdmin);
        
        return Result<UserProfileDto>.Success(dto);
    }

    public async Task<Result<List<UserProfileDto>>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new UserProfileDto(u.Id, u.Username, u.Email, u.UserRoles.Any(ur => ur.Role.Name == "Admin")))
            .ToListAsync(cancellationToken);

        return Result<List<UserProfileDto>>.Success(users);
    }
}
