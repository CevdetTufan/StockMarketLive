using Microsoft.EntityFrameworkCore;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Common;
using StockMarketLive.Domain.Entities;
using StockMarketLive.Infrastructure.Persistence;
using StockMarketLive.Application.DTOs.Auth;

namespace StockMarketLive.Infrastructure.Services;

public class AuthService(AppDbContext context, IPasswordHasher passwordHasher, IJwtProvider jwtProvider) : IAuthService
{
    private readonly AppDbContext _context = context;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

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
        var roles = user.UserRoles.Select(ur => new RoleDto(ur.Role.Id, ur.Role.Name)).ToList();

        var dto = new UserProfileDto(user.Id, user.Username, user.Email, isAdmin, roles);
        
        return Result<UserProfileDto>.Success(dto);
    }

    public async Task<Result<List<UserProfileDto>>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync(cancellationToken);

        var userDtos = users.Select(u => new UserProfileDto(
            u.Id, 
            u.Username, 
            u.Email, 
            u.UserRoles.Any(ur => ur.Role.Name == "Admin"),
			[.. u.UserRoles.Select(ur => new RoleDto(ur.Role.Id, ur.Role.Name))]
		)).ToList();

		return Result<List<UserProfileDto>>.Success(userDtos);
    }
    public async Task<Result<Guid>> CreateRoleAsync(string name, CancellationToken cancellationToken = default)
    {
        if (await _context.Roles.AnyAsync(r => r.Name == name, cancellationToken))
        {
            return Result<Guid>.Failure("Bu isimde bir rol zaten mevcut.");
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(role.Id);
    }

    public async Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .Select(r => new RoleDto(r.Id, r.Name))
            .ToListAsync(cancellationToken);

        return Result<List<RoleDto>>.Success(roles);
    }

    public async Task<Result<List<PermissionDto>>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _context.Permissions
            .AsNoTracking()
            .Select(p => new PermissionDto(p.Id, p.SystemName, p.Description))
            .ToListAsync(cancellationToken);

        return Result<List<PermissionDto>>.Success(permissions);
    }

    public async Task<Result<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.Include(u => u.UserRoles).SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null) return Result<bool>.Failure("Kullanıcı bulunamadı.");

        var role = await _context.Roles.FindAsync([roleId], cancellationToken);
        if (role == null) return Result<bool>.Failure("Rol bulunamadı.");

        if (user.UserRoles.Any(ur => ur.RoleId == roleId))
        {
            return Result<bool>.Failure("Kullanıcı zaten bu role sahip.");
        }

        user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.Include(r => r.RolePermissions).SingleOrDefaultAsync(r => r.Id == roleId, cancellationToken);
        if (role == null) return Result<bool>.Failure("Rol bulunamadı.");

        var permission = await _context.Permissions.FindAsync([permissionId], cancellationToken);
        if (permission == null) return Result<bool>.Failure("Yetki bulunamadı.");

        if (role.RolePermissions.Any(rp => rp.PermissionId == permissionId))
        {
            return Result<bool>.Failure("Rol zaten bu yetkiye sahip.");
        }

        role.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
