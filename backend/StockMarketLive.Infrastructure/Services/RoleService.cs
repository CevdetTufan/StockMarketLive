using Microsoft.EntityFrameworkCore;
using StockMarketLive.Application.DTOs.Auth;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Common;
using StockMarketLive.Domain.Constants;
using StockMarketLive.Domain.Entities;
using StockMarketLive.Infrastructure.Persistence;

namespace StockMarketLive.Infrastructure.Services;

public class RoleService(AppDbContext context) : IRoleService
{
    private readonly AppDbContext _context = context;

    public async Task<Result<Guid>> CreateRoleAsync(string name, CancellationToken ct = default)
    {
        if (await _context.Roles.AnyAsync(r => r.Name == name, ct))
        {
            return Result<Guid>.Failure(AppConstants.ErrorCodes.Role.RoleAlreadyExists);
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(ct);

        return Result<Guid>.Success(role.Id);
    }

    public async Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken ct = default)
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .Select(r => new RoleDto(r.Id, r.Name))
            .ToListAsync(ct);

        return Result<List<RoleDto>>.Success(roles);
    }

    public async Task<Result<List<PermissionDto>>> GetPermissionsAsync(CancellationToken ct = default)
    {
        var permissions = await _context.Permissions
            .AsNoTracking()
            .Select(p => new PermissionDto(p.Id, p.SystemName, p.Description))
            .ToListAsync(ct);

        return Result<List<PermissionDto>>.Success(permissions);
    }

    public async Task<Result<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var user = await _context.Users.Include(u => u.UserRoles).SingleOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null) return Result<bool>.Failure(AppConstants.ErrorCodes.Auth.UserNotFound);

        var role = await _context.Roles.FindAsync([roleId], ct);
        if (role == null) return Result<bool>.Failure(AppConstants.ErrorCodes.Role.RoleNotFound);

        if (user.UserRoles.Any(ur => ur.RoleId == roleId))
        {
            return Result<bool>.Failure(AppConstants.ErrorCodes.Role.UserAlreadyHasRole);
        }

        user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
    {
        var role = await _context.Roles.Include(r => r.RolePermissions).SingleOrDefaultAsync(r => r.Id == roleId, ct);
        if (role == null) return Result<bool>.Failure(AppConstants.ErrorCodes.Role.RoleNotFound);

        var permission = await _context.Permissions.FindAsync([permissionId], ct);
        if (permission == null) return Result<bool>.Failure(AppConstants.ErrorCodes.Role.PermissionNotFound);

        if (role.RolePermissions.Any(rp => rp.PermissionId == permissionId))
        {
            return Result<bool>.Failure(AppConstants.ErrorCodes.Role.RoleAlreadyHasPermission);
        }

        role.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
