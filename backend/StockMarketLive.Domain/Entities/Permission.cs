namespace StockMarketLive.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; }
    public string SystemName { get; set; } = string.Empty; // e.g. "Trade.Execute", "Signals.View"
    public string Description { get; set; } = string.Empty;

    // Navigation property
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
