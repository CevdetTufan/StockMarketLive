namespace StockMarketLive.Application.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using StockMarket.Shared.Contracts.Events;

public sealed class OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger) : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        if (context.Message is null)
        {
            logger.LogWarning("Received null OrderCreatedEvent.");
            return Task.CompletedTask;
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

        return Task.CompletedTask;
    }
}
