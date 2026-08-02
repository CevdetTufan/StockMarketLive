using StockMarketLive.Domain.Common;
using StockMarketLive.Application.DTOs.Auth;

namespace StockMarketLive.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(string username, string password, CancellationToken ct = default);
    Task<Result<Guid>> RegisterAsync(string username, string email, string password, CancellationToken ct = default);
    Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken ct = default);
    Task<Result<List<UserProfileDto>>> GetUsersAsync(CancellationToken ct = default);
}
