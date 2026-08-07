namespace StockMarketLive.Application.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using StockMarket.Shared.Contracts.Events;

using StockMarketLive.Application.Interfaces;

public sealed class StockPriceUpdatedEventConsumer(
    ILogger<StockPriceUpdatedEventConsumer> logger,
    ILiveStockService liveStockService) : IConsumer<StockPriceUpdatedEvent>
{
    public async Task Consume(ConsumeContext<StockPriceUpdatedEvent> context)
    {
        if (context.Message is null)
        {
            logger.LogWarning("Received null StockPriceUpdatedEvent.");
            return;
        }

        var message = context.Message;
        
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "StockPriceUpdatedEvent received: Symbol={Symbol}, CurrentPrice={CurrentPrice}, ChangeRate={ChangeRate}, UpdatedAt={UpdatedAt}",
                message.Symbol, 
                message.CurrentPrice, 
                message.ChangeRate, 
                message.UpdatedAt);
        }

        await liveStockService.BroadcastStockPriceUpdatedAsync(message, context.CancellationToken);
    }
}
