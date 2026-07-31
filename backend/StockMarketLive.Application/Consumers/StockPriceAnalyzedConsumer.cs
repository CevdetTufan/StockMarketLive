namespace StockMarketLive.Application.Consumers;

using MassTransit;
using StockMarketLive.Domain.Events;
using StockMarketLive.Application.Interfaces;

/// <summary>
/// RabbitMQ'dan gelen 'StockPriceAnalyzedEvent' olaylarını (event) dinleyen tüketici.
/// Primary Constructor syntax'ı kullanılmıştır.
/// </summary>
public sealed class StockPriceAnalyzedConsumer(ILiveStockService liveStockService) : IConsumer<StockPriceAnalyzedEvent>
{
    public async Task Consume(ConsumeContext<StockPriceAnalyzedEvent> context)
    {
        // Event'i al ve doğrudan canlı servise (SignalR sarmalayıcısına) ilet.
        await liveStockService.BroadcastStockUpdateAsync(context.Message, context.CancellationToken);
    }
}
