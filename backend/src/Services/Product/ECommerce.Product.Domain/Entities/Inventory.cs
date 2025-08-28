using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Domain.Entities;

public class Inventory : BaseAuditableEntity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int AvailableQuantity { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public DateTime LastUpdated { get; private set; }
    
    // Navigation properties
    public Product Product { get; private set; } = null!;
    
    // Private constructor for EF Core
    private Inventory() { }
    
    public Inventory(Guid productId, int quantity, string location)
    {
        ProductId = productId;
        Quantity = quantity;
        ReservedQuantity = 0;
        AvailableQuantity = quantity;
        Location = location;
        LastUpdated = DateTime.UtcNow;
    }
    
    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        AvailableQuantity = quantity - ReservedQuantity;
        LastUpdated = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ReserveStock(int quantity)
    {
        if (quantity <= AvailableQuantity)
        {
            ReservedQuantity += quantity;
            AvailableQuantity = Quantity - ReservedQuantity;
            LastUpdated = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            throw new InvalidOperationException("Insufficient available stock");
        }
    }
    
    public void ReleaseReservedStock(int quantity)
    {
        if (quantity <= ReservedQuantity)
        {
            ReservedQuantity -= quantity;
            AvailableQuantity = Quantity - ReservedQuantity;
            LastUpdated = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            throw new InvalidOperationException("Cannot release more than reserved stock");
        }
    }
    
    public void AdjustStock(int quantityChange, string reason)
    {
        Quantity += quantityChange;
        AvailableQuantity = Quantity - ReservedQuantity;
        LastUpdated = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateLocation(string location)
    {
        Location = location;
        LastUpdated = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool HasStock() => AvailableQuantity > 0;
    public bool IsLowStock(int threshold = 10) => AvailableQuantity <= threshold;
}
