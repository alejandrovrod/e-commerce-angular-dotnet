using ECommerce.BuildingBlocks.EventBus.Events;

namespace ECommerce.BuildingBlocks.EventBus.Abstractions;

public interface IIntegrationEventHandler<in T> where T : IntegrationEvent
{
    Task HandleAsync(T @event, CancellationToken cancellationToken = default);
}

