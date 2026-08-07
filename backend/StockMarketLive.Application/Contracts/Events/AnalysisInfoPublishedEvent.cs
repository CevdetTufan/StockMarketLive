#pragma warning disable IDE0130
namespace StockMarket.Shared.Contracts.Events;

public sealed record AnalysisInfoPublishedEvent(
    Guid AnalysisId,
    string Symbol,
    string Recommendation,
    double Score,
    DateTime PublishedAt
);
