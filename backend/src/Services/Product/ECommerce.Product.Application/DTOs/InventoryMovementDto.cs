namespace ECommerce.Product.Application.DTOs;

public class InventoryMovementDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string MovementType { get; set; } = string.Empty; // 'adjustment', 'reservation', 'release', 'sale', 'return'
    public int Quantity { get; set; }
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public Guid? OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
}
