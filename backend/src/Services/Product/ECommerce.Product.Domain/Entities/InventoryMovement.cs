using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Domain.Entities;

public class InventoryMovement : BaseAuditableEntity
{
    public Guid ProductId { get; private set; }
    public string MovementType { get; private set; } = string.Empty; // 'adjustment', 'reservation', 'release', 'sale', 'return'
    public int Quantity { get; private set; }
    public int PreviousQuantity { get; private set; }
    public int NewQuantity { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public Guid? UserId { get; private set; }
    public string? UserName { get; private set; }
    public Guid? OrderId { get; private set; }
    
    // Navigation properties
    public Product Product { get; private set; } = null!;
    
    // Private constructor for EF Core
    private InventoryMovement() { }
    
    public InventoryMovement(
        Guid productId,
        string movementType,
        int quantity,
        int previousQuantity,
        int newQuantity,
        string reason,
        string? notes = null,
        Guid? userId = null,
        string? userName = null,
        Guid? orderId = null)
    {
        ProductId = productId;
        MovementType = movementType;
        Quantity = quantity;
        PreviousQuantity = previousQuantity;
        NewQuantity = newQuantity;
        Reason = reason;
        Notes = notes;
        UserId = userId;
        UserName = userName;
        OrderId = orderId;
    }
    
    public static InventoryMovement CreateAdjustment(
        Guid productId,
        int quantityChange,
        int previousQuantity,
        int newQuantity,
        string reason,
        string? notes = null,
        Guid? userId = null,
        string? userName = null)
    {
        return new InventoryMovement(
            productId,
            "adjustment",
            quantityChange,
            previousQuantity,
            newQuantity,
            reason,
            notes,
            userId,
            userName
        );
    }
    
    public static InventoryMovement CreateSale(
        Guid productId,
        int quantitySold,
        int previousQuantity,
        int newQuantity,
        Guid orderId,
        string? notes = null,
        Guid? userId = null,
        string? userName = null)
    {
        return new InventoryMovement(
            productId,
            "sale",
            -quantitySold, // Negative quantity for sales
            previousQuantity,
            newQuantity,
            "Venta de producto",
            notes,
            userId,
            userName,
            orderId
        );
    }
    
    public static InventoryMovement CreateReservation(
        Guid productId,
        int quantityReserved,
        int previousQuantity,
        int newQuantity,
        string reason,
        Guid? orderId = null,
        string? notes = null,
        Guid? userId = null,
        string? userName = null)
    {
        return new InventoryMovement(
            productId,
            "reservation",
            -quantityReserved, // Negative quantity for reservations
            previousQuantity,
            newQuantity,
            reason,
            notes,
            userId,
            userName,
            orderId
        );
    }
}
