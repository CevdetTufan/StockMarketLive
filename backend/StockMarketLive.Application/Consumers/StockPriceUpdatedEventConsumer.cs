namespace StockMarketLive.Application.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using StockMarket.Shared.Contracts.Events;

public sealed class StockPriceUpdatedEventConsumer(ILogger<StockPriceUpdatedEventConsumer> logger) : IConsumer<StockPriceUpdatedEvent>
{
    public Task Consume(ConsumeContext<StockPriceUpdatedEvent> context)
    {
        if (context.Message is null)
        {
            logger.LogWarning("Received null StockPriceUpdatedEvent.");
            return Task.CompletedTask;
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

        return Task.CompletedTask;
    }
}
