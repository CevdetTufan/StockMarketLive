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
                var host = configuration["RABBITMQ_HOST"];
                var user = configuration["RABBITMQ_USERNAME"];
                var pass = configuration["RABBITMQ_PASSWORD"];

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    throw new InvalidOperationException("RabbitMQ configuration (RABBITMQ_HOST, RABBITMQ_USERNAME, RABBITMQ_PASSWORD) is missing.");
                }

                cfg.Host(host, "/", h =>
                {
                    h.Username(user);
                    h.Password(pass);
                });

                // ConfigureEndpoints will automatically create queues and subscribe to exchanges based on consumer names
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
