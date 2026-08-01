using Microsoft.EntityFrameworkCore;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Entities;

namespace StockMarketLive.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        // Otomatik Migration Uygulama (Veritabanı yoksa oluşturur, varsa günceller)
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration Error: {ex.Message}");
            throw; // PostgreSQL'e hiç bağlanılamıyorsa hatayı yukarı fırlatır
        }

        // Add Admin Role
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role { Id = Guid.NewGuid(), Name = "Admin" };
            context.Roles.Add(adminRole);
        }

        // Add Admin User
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@stockmarketlive.com",
                PasswordHash = passwordHasher.HashPassword("123456"),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            context.Users.Add(adminUser);

            // Assign Admin Role to User
            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
        }

        await context.SaveChangesAsync();
    }
}
