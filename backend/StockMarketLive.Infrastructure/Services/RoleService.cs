using Microsoft.EntityFrameworkCore;
using StockMarketLive.Application.DTOs.Auth;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Common;
using StockMarketLive.Domain.Entities;
using StockMarketLive.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StockMarketLive.Infrastructure.Services;

public class RoleService(AppDbContext context) : IRoleService
{
    private readonly AppDbContext _context = context;

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
