namespace StockMarketLive.Application.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using StockMarket.Shared.Contracts.Events;

using StockMarketLive.Application.Interfaces;

public sealed class OrderCreatedEventConsumer(
    ILogger<OrderCreatedEventConsumer> logger,
    ILiveStockService liveStockService) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        if (context.Message is null)
        {
            logger.LogWarning("Received null OrderCreatedEvent.");
            return;
        }

        var message = context.Message;
        
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "OrderCreatedEvent received: OrderId={OrderId}, Symbol={Symbol}, Price={Price}, Quantity={Quantity}, Side={Side}, CreatedAt={CreatedAt}",
                message.OrderId, 
                message.Symbol, 
                message.Price, 
                message.Quantity, 
                message.Side, 
                message.CreatedAt);
        }

        await liveStockService.BroadcastOrderCreatedAsync(message, context.CancellationToken);
    }
}
