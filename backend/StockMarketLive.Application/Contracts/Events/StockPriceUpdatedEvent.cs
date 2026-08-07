#pragma warning disable IDE0130
namespace StockMarket.Shared.Contracts.Events;

public sealed record StockPriceUpdatedEvent(
    string Symbol,
    decimal CurrentPrice,
    decimal ChangeRate,
    DateTime UpdatedAt
);
