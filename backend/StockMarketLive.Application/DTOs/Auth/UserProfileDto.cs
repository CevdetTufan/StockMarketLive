namespace StockMarketLive.Application.DTOs.Auth;

public record UserProfileDto(Guid Id, string Username, string Email, bool IsAdmin);
