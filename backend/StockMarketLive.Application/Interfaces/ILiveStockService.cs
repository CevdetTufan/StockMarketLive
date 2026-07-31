namespace StockMarketLive.Application.Interfaces;

using StockMarketLive.Domain.Events;

/// <summary>
/// Domain'den veya Message Broker'dan gelen canlı borsa verilerini 
/// dış dünyaya (Web/SignalR) iletmek için soyutlama arayüzü.
/// Application katmanının dış dünya (SignalR) bağımlılığını keser.
/// </summary>
public interface ILiveStockService
{
    Task BroadcastStockUpdateAsync(StockPriceAnalyzedEvent stockEvent, CancellationToken ct);
}
