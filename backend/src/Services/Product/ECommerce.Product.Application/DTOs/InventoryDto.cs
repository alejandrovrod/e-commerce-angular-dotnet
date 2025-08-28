namespace ECommerce.Product.Application.DTOs;

public class InventoryDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public ProductDto Product { get; set; } = new();
}
