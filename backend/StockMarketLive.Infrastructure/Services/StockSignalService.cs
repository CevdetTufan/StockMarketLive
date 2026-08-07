using StockMarket.Shared.Contracts.Events;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Entities;
using StockMarketLive.Infrastructure.Persistence;

namespace StockMarketLive.Infrastructure.Services;

public sealed class StockSignalService(AppDbContext context, ILiveStockService liveStockService) : IStockSignalService
{
    public async Task ProcessSignalAsync(AnalysisInfoPublishedEvent stockEvent, CancellationToken ct = default)
    {
        var signal = new StockSignal
        {
            Id = Guid.NewGuid(),
            Symbol = stockEvent.Symbol,
            Recommendation = stockEvent.Recommendation,
            Score = stockEvent.Score,
            PublishedAt = stockEvent.PublishedAt
        };
        
        context.StockSignals.Add(signal);
        await context.SaveChangesAsync(ct);

        await liveStockService.BroadcastStockUpdateAsync(stockEvent, ct);
    }
}
