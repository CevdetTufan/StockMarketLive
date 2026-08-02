namespace StockMarketLive.Application.Consumers;

using MassTransit;
using StockMarketLive.Domain.Events;
using StockMarketLive.Application.Interfaces;
using System.Threading.Tasks;

public sealed class StockPriceAnalyzedConsumer(IStockSignalService stockSignalService) : IConsumer<StockPriceAnalyzedEvent>
{
    public async Task Consume(ConsumeContext<StockPriceAnalyzedEvent> context)
    {
        await stockSignalService.ProcessSignalAsync(context.Message, context.CancellationToken);
    }
}
