using Microsoft.EntityFrameworkCore;
using StockMarketLive.Domain.Entities;

namespace StockMarketLive.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<StockSignal> StockSignals => Set<StockSignal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Username).IsRequired().HasMaxLength(50);
            b.Property(u => u.Email).IsRequired().HasMaxLength(100);
            b.Property(u => u.PasswordHash).IsRequired();
            b.HasIndex(u => u.Username).IsUnique();
            b.HasIndex(u => u.Email).IsUnique();
        });

        // Role Configuration
        modelBuilder.Entity<Role>(b =>
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.Name).IsRequired().HasMaxLength(50);
            b.HasIndex(r => r.Name).IsUnique();
        });

        // UserRole (Many-to-Many Bridge)
        modelBuilder.Entity<UserRole>(b =>
        {
            b.HasKey(ur => new { ur.UserId, ur.RoleId });

            b.HasOne(ur => ur.User)
             .WithMany(u => u.UserRoles)
             .HasForeignKey(ur => ur.UserId);

            b.HasOne(ur => ur.Role)
             .WithMany(r => r.UserRoles)
             .HasForeignKey(ur => ur.RoleId);
        });

        // Permission Configuration
        modelBuilder.Entity<Permission>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.SystemName).IsRequired().HasMaxLength(100);
            b.HasIndex(p => p.SystemName).IsUnique();
        });

        // RolePermission (Many-to-Many Bridge)
        modelBuilder.Entity<RolePermission>(b =>
        {
            b.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            b.HasOne(rp => rp.Role)
             .WithMany(r => r.RolePermissions)
             .HasForeignKey(rp => rp.RoleId);

            b.HasOne(rp => rp.Permission)
             .WithMany(p => p.RolePermissions)
             .HasForeignKey(rp => rp.PermissionId);
        });

        // StockSignal Configuration
        modelBuilder.Entity<StockSignal>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Symbol).IsRequired().HasMaxLength(20);
            b.Property(s => s.Recommendation).IsRequired().HasMaxLength(50);
            b.HasIndex(s => s.Symbol); // Optimize queries by symbol
            b.HasIndex(s => s.PublishedAt); // Optimize time-series queries
        });
    }
}
