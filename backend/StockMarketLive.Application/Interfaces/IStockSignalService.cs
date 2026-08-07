using StockMarket.Shared.Contracts.Events;

namespace StockMarketLive.Application.Interfaces;

public interface IStockSignalService
{
    Task ProcessSignalAsync(AnalysisInfoPublishedEvent stockEvent, CancellationToken ct = default);
}
