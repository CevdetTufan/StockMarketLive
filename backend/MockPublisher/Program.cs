using MassTransit;
using Microsoft.Extensions.Configuration;
using StockMarket.Shared.Contracts.Events;

Console.WriteLine("Mock Publisher Started.");

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<StockPriceUpdatedEvent>() // Using an event class just as a type anchor for UserSecrets
    .AddEnvironmentVariables()
    .Build();

// Retrieve RabbitMQ URL from user secrets or environment variables
var rabbitUrl = configuration["RABBITMQ_URL"];

if (string.IsNullOrEmpty(rabbitUrl))
{
    Console.WriteLine("ERROR: RABBITMQ_URL is not set in User Secrets or Environment Variables.");
    Console.WriteLine("To set it, run: dotnet user-secrets set \"RABBITMQ_URL\" \"amqps://...\"");
    return;
}

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host(new Uri(rabbitUrl));
});

await bus.StartAsync();
Console.WriteLine("Connected to RabbitMQ. Publishing events...");

var rnd = new Random();
var symbols = new[] { "AAPL", "MSFT", "GOOGL", "TSLA", "AMZN", "NVDA" };
var sides = new[] { "BUY", "SELL" };
var recommendations = new[] { "Buy", "Sell", "Hold", "Strong Buy", "Strong Sell" };

try
{
    while (true)
    {
        var symbol = symbols[rnd.Next(symbols.Length)];
        var basePrice = symbol switch
        {
            "AAPL" => 150m,
            "MSFT" => 320m,
            "GOOGL" => 2800m,
            "TSLA" => 900m,
            "AMZN" => 3300m,
            "NVDA" => 600m,
            _ => 100m
        };
        
        var currentPrice = basePrice + (decimal)(rnd.NextDouble() * 10 - 5);
        var changeRate = (decimal)(rnd.NextDouble() * 4 - 2);

        // Publish Price Update
        await bus.Publish(new StockPriceUpdatedEvent(
            symbol,
            Math.Round(currentPrice, 2),
            Math.Round(changeRate, 2),
            DateTime.UtcNow
        ));
        Console.WriteLine($"[Price] {symbol} => ${currentPrice:F2}");

        // Occasional Order
        if (rnd.Next(10) > 6)
        {
            var side = sides[rnd.Next(sides.Length)];
            var qty = rnd.Next(1, 100);
            await bus.Publish(new OrderCreatedEvent(
                Guid.NewGuid(),
                symbol,
                Math.Round(currentPrice, 2),
                qty,
                side,
                DateTime.UtcNow
            ));
            Console.WriteLine($"[Order] {side} {qty} {symbol} @ ${currentPrice:F2}");
        }

        // Occasional AI Analysis
        if (rnd.Next(10) > 7)
        {
            var recommendation = recommendations[rnd.Next(recommendations.Length)];
            var score = rnd.NextDouble() * 100;
            await bus.Publish(new AnalysisInfoPublishedEvent(
                Guid.NewGuid(),
                symbol,
                recommendation,
                score,
                DateTime.UtcNow
            ));
            Console.WriteLine($"[Analysis] {symbol} => {recommendation} (Score: {score:F2})");
        }

        await Task.Delay(rnd.Next(1000, 3000));
    }
}
finally
{
    await bus.StopAsync();
}
