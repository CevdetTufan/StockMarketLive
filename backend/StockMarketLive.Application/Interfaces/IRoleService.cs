using StockMarketLive.Application.DTOs.Auth;
using StockMarketLive.Domain.Common;

namespace StockMarketLive.Application.Interfaces;

public interface IRoleService
{
    Task<Result<Guid>> CreateRoleAsync(string name, CancellationToken ct = default);
    Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken ct = default);
    Task<Result<List<PermissionDto>>> GetPermissionsAsync(CancellationToken ct = default);
    Task<Result<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken ct = default);
    Task<Result<bool>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken ct = default);
}
