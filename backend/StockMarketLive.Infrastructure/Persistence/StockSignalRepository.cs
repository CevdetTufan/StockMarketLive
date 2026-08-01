using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Entities;

namespace StockMarketLive.Infrastructure.Persistence;

public class StockSignalRepository : IStockSignalRepository
{
    private readonly AppDbContext _context;

    public StockSignalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StockSignal signal, CancellationToken cancellationToken = default)
    {
        _context.StockSignals.Add(signal);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
