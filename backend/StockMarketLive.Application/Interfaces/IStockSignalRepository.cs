using StockMarketLive.Domain.Entities;

namespace StockMarketLive.Application.Interfaces;

public interface IStockSignalRepository
{
    Task AddAsync(StockSignal signal, CancellationToken cancellationToken = default);
}
