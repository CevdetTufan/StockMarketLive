namespace StockMarketLive.Domain.Constants;

/// <summary>
/// Projedeki sihirli metinlerin (magic strings) barındığı sabitler sınıfı. (Sıfır Hardcode kuralı)
/// </summary>
public static class AppConstants
{
    public const string CorsPolicyName = "AllowFrontend";
    
    public static class RabbitMq
    {
        public const string ExchangeName = "stock.market.exchange";
        public const string QueueName = "stock.market.live.queue";
    }

    public static class SignalR
    {
        public const string HubEndpoint = "/hubs/stock";
        public const string ReceiveEventName = "ReceiveStockUpdate";
    }
}
