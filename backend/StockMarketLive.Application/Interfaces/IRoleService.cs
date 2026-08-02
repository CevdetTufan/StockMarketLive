using StockMarketLive.Application.DTOs.Auth;
using StockMarketLive.Domain.Common;

namespace StockMarketLive.Application.Interfaces;

public interface IRoleService
{
    Task<Result<Guid>> CreateRoleAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<PermissionDto>>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
}
