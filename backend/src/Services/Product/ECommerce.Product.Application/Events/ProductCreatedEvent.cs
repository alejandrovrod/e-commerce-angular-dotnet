using ECommerce.BuildingBlocks.EventBus.Events;

namespace ECommerce.Product.Application.Events;

public record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    string SKU,
    string Brand,
    Guid CategoryId,
    decimal Price,
    bool IsDigital,
    bool RequiresShipping,
    DateTime CreatedAt
) : IntegrationEvent;

public record ProductUpdatedEvent(
    Guid ProductId,
    string Name,
    string SKU,
    Guid CategoryId,
    string Brand,
    decimal Price,
    DateTime UpdatedAt
) : IntegrationEvent;

public record ProductDeletedEvent(
    Guid ProductId,
    string Name,
    string SKU,
    DateTime DeletedAt
) : IntegrationEvent;

public record InventoryUpdatedEvent(
    Guid ProductId,
    string ProductName,
    int OldStockQuantity,
    int NewStockQuantity,
    int ReservedQuantity,
    string Status,
    DateTime UpdatedAt
) : IntegrationEvent;

public record ReviewAddedEvent(
    Guid ReviewId,
    Guid ProductId,
    string ProductName,
    Guid UserId,
    string UserName,
    decimal Rating,
    string Title,
    DateTime CreatedAt
) : IntegrationEvent;

public record CategoryCreatedEvent(
    Guid CategoryId,
    string Name,
    string Slug,
    Guid? ParentId,
    DateTime CreatedAt
) : IntegrationEvent;

public record CategoryUpdatedEvent(
    Guid CategoryId,
    string Name,
    string Slug,
    Guid? ParentId,
    DateTime UpdatedAt
) : IntegrationEvent;

public record BrandCreatedEvent(
    Guid BrandId,
    string Name,
    string Slug,
    string? Country,
    DateTime CreatedAt
) : IntegrationEvent;

public record BrandUpdatedEvent(
    Guid BrandId,
    string Name,
    string Slug,
    string? Country,
    DateTime UpdatedAt
) : IntegrationEvent;
