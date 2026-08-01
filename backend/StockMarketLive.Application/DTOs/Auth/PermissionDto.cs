using System;

namespace StockMarketLive.Application.DTOs.Auth;

public record PermissionDto(Guid Id, string SystemName, string Description);
