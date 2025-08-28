using ECommerce.BuildingBlocks.EventBus.Events;

namespace ECommerce.Product.Application.Interfaces;

public interface IEventService
{
    Task PublishAsync<T>(T @event) where T : IntegrationEvent;
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T : IntegrationEvent;
}

