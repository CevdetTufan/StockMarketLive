namespace StockMarketLive.Infrastructure;

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StockMarketLive.Application.Consumers;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Constants;
using StockMarketLive.Infrastructure.Persistence;
using StockMarketLive.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register Options
        services.Configure<StockMarketLive.Application.Settings.JwtSettings>(
            configuration.GetSection(StockMarketLive.Application.Settings.JwtSettings.SectionName));

        // Add Auth Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IStockSignalService, StockSignalService>();

		services.AddMassTransit(x =>
		{
			x.AddConsumer<OrderCreatedEventConsumer>();
			x.AddConsumer<AnalysisInfoPublishedEventConsumer>();
			x.AddConsumer<StockPriceUpdatedEventConsumer>();

			x.UsingRabbitMq((context, cfg) =>
			{
				var url = configuration["RABBITMQ_URL"];
				if (!string.IsNullOrEmpty(url))
				{
					cfg.Host(new Uri(url));
				}
				else
				{
					var host = configuration["RABBITMQ_HOST"];
					var user = configuration["RABBITMQ_USERNAME"];
					var pass = configuration["RABBITMQ_PASSWORD"];
					var vhost = configuration["RABBITMQ_VHOST"] ?? "/";

					if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
					{
						throw new InvalidOperationException("RabbitMQ configuration is missing (RABBITMQ_URL or HOST/USER/PASS).");
					}

					cfg.Host(host, vhost, h =>
					{
						h.Username(user);
						h.Password(pass);
					});
				}


				cfg.ReceiveEndpoint("order-created-event", e =>
				{
					e.Durable = false;
					e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
				});

				cfg.ReceiveEndpoint("analysis-info-published-event", e =>
				{
					e.Durable = false;
					e.ConfigureConsumer<AnalysisInfoPublishedEventConsumer>(context);
				});

				cfg.ReceiveEndpoint("stock-price-updated-event", e =>
				{
					e.Durable = false; 

					e.ConfigureConsumer<StockPriceUpdatedEventConsumer>(context);
				});
			});
		});

		return services;
    }
}
