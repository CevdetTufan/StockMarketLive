namespace StockMarketLive.Application.Consumers;

using MassTransit;
using Microsoft.Extensions.Logging;
using StockMarket.Shared.Contracts.Events;
using StockMarketLive.Application.Interfaces;

public sealed class AnalysisInfoPublishedEventConsumer(
    ILogger<AnalysisInfoPublishedEventConsumer> logger,
    IStockSignalService stockSignalService) : IConsumer<AnalysisInfoPublishedEvent>
{
    public async Task Consume(ConsumeContext<AnalysisInfoPublishedEvent> context)
    {
        if (context.Message is null)
        {
            logger.LogWarning("Received null AnalysisInfoPublishedEvent.");
            return;
        }

        var message = context.Message;
        
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "AnalysisInfoPublishedEvent received: AnalysisId={AnalysisId}, Symbol={Symbol}, Recommendation={Recommendation}, Score={Score}, PublishedAt={PublishedAt}",
                message.AnalysisId, 
                message.Symbol, 
                message.Recommendation, 
                message.Score, 
                message.PublishedAt);
        }

        await stockSignalService.ProcessSignalAsync(message, context.CancellationToken);
    }
}
