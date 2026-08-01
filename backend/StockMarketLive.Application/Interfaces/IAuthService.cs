using StockMarketLive.Domain.Common;
using StockMarketLive.Application.DTOs.Auth;

namespace StockMarketLive.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<Result<Guid>> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default);
    Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<UserProfileDto>>> GetUsersAsync(CancellationToken cancellationToken = default);

    // RBAC Methods
    Task<Result<Guid>> CreateRoleAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<PermissionDto>>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
}
