namespace StockMarketLive.Domain.Events;

/// <summary>
/// Private proje (Publisher) tarafından fırlatılacak ve bu proje (Consumer) tarafından dinlenecek
/// RabbitMQ mesaj sözleşmesi (Contract).
/// </summary>
public sealed record StockPriceAnalyzedEvent
{
    public required string Symbol { get; init; }
    public required decimal Price { get; init; }
    
    /// <summary>
    /// 0: Hold, 1: Buy, 2: Sell
    /// </summary>
    public required int Signal { get; init; }
    
    public string? AiReason { get; init; }
    public required DateTime Timestamp { get; init; }
}
