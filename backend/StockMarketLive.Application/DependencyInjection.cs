using Microsoft.Extensions.DependencyInjection;
using StockMarketLive.Application.Interfaces;
using FluentValidation;

namespace StockMarketLive.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
