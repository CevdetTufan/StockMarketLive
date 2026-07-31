namespace StockMarketLive.Infrastructure;

using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StockMarketLive.Application.Consumers;
using StockMarketLive.Domain.Constants;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<StockPriceAnalyzedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                // Güvenlik Kuralı: Şifreler ve URL'ler koda yazılmaz, IConfiguration üzerinden (.env veya user-secrets) okunur.
                var rabbitMqUrl = configuration.GetConnectionString("RabbitMq");
                
                if (string.IsNullOrEmpty(rabbitMqUrl))
                {
                    throw new InvalidOperationException("RabbitMQ ConnectionString is missing in configuration or secrets.");
                }

                cfg.Host(new Uri(rabbitMqUrl));

                // Profesyonel Exchange (Pub/Sub) Stratejisi
                cfg.ReceiveEndpoint(AppConstants.RabbitMq.QueueName, e =>
                {
                    e.ConfigureConsumer<StockPriceAnalyzedConsumer>(context);
                    
                    // Publisher (Mevcut proje) bu exchange'e atacak, biz buradan dinleyeceğiz.
                    e.Bind(AppConstants.RabbitMq.ExchangeName, x =>
                    {
                        x.ExchangeType = "fanout";
                    });
                });
            });
        });

        return services;
    }
}
