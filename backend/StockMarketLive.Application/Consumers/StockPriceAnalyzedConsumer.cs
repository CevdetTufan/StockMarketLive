namespace StockMarketLive.Application.Consumers;

using MassTransit;
using StockMarketLive.Domain.Events;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Entities;

/// <summary>
/// RabbitMQ'dan gelen 'StockPriceAnalyzedEvent' olaylarını (event) dinleyen tüketici.
/// Primary Constructor syntax'ı kullanılmıştır.
/// </summary>
public sealed class StockPriceAnalyzedConsumer(ILiveStockService liveStockService, IStockSignalRepository repository) : IConsumer<StockPriceAnalyzedEvent>
{
    public async Task Consume(ConsumeContext<StockPriceAnalyzedEvent> context)
    {
        var ev = context.Message;
        
        // Veritabanına kaydet
        var signal = new StockSignal
        {
            Id = Guid.NewGuid(),
            Symbol = ev.Symbol,
            Price = ev.Price,
            Signal = ev.Signal,
            AiReason = ev.AiReason,
            Timestamp = ev.Timestamp
        };
        
        await repository.AddAsync(signal, context.CancellationToken);

        // Event'i al ve doğrudan canlı servise (SignalR sarmalayıcısına) ilet.
        await liveStockService.BroadcastStockUpdateAsync(context.Message, context.CancellationToken);
    }
}
