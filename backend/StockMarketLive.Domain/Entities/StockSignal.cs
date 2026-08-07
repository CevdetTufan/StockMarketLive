namespace StockMarketLive.Domain.Entities;

public class StockSignal
{
    public Guid Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public double Score { get; set; }
    public DateTime PublishedAt { get; set; }
}
