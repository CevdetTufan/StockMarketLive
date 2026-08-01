namespace StockMarketLive.Domain.Entities;

public class StockSignal
{
    public Guid Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Signal { get; set; } // 0: Hold, 1: Buy, -1: Sell
    public string? AiReason { get; set; }
    public DateTime Timestamp { get; set; }
}
