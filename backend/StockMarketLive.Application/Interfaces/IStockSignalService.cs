using StockMarketLive.Domain.Events;

namespace StockMarketLive.Application.Interfaces;

public interface IStockSignalService
{
    Task ProcessSignalAsync(StockPriceAnalyzedEvent stockEvent, CancellationToken ct = default);
}
