#pragma warning disable IDE0130
namespace StockMarket.Shared.Contracts.Events;

public sealed record OrderCreatedEvent(
    Guid OrderId,
    string Symbol,
    decimal Price,
    decimal Quantity,
    string Side,
    DateTime CreatedAt
);
