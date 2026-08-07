#pragma warning disable IDE0130
namespace StockMarket.Shared.Contracts.Events;

public sealed record AnalysisInfoPublishedEvent(
    Guid AnalysisId,
    string Symbol,
    string Recommendation,
    double Score,
    DateTime PublishedAt
);

public sealed record OrderCreatedEvent(
    Guid OrderId,
    string Symbol,
    decimal Price,
    decimal Quantity,
    string Side,
    DateTime CreatedAt
);

public sealed record StockPriceUpdatedEvent(
    string Symbol,
    decimal CurrentPrice,
    decimal ChangeRate,
    DateTime UpdatedAt
);
