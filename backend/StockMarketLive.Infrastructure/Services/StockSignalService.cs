using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Entities;
using StockMarketLive.Domain.Events;
using StockMarketLive.Infrastructure.Persistence;

namespace StockMarketLive.Infrastructure.Services;

public class StockSignalService(AppDbContext context, ILiveStockService liveStockService) : IStockSignalService
{
    public async Task ProcessSignalAsync(StockPriceAnalyzedEvent stockEvent, CancellationToken ct = default)
    {
        var signal = new StockSignal
        {
            Id = Guid.NewGuid(),
            Symbol = stockEvent.Symbol,
            Price = stockEvent.Price,
            Signal = stockEvent.Signal,
            AiReason = stockEvent.AiReason,
            Timestamp = stockEvent.Timestamp
        };
        
        context.StockSignals.Add(signal);
        await context.SaveChangesAsync(ct);

        await liveStockService.BroadcastStockUpdateAsync(stockEvent, ct);
    }
}
