using ECommerce.BuildingBlocks.EventBus.Events;

namespace ECommerce.User.Application.Events;

public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt
) : IntegrationEvent;
