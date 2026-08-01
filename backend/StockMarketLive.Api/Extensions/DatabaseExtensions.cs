using StockMarketLive.Application.Interfaces;
using StockMarketLive.Infrastructure.Persistence;

namespace StockMarketLive.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        
        await DataSeeder.SeedAsync(context, passwordHasher);
    }
}
