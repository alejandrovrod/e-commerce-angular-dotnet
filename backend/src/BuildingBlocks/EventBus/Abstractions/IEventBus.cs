using ECommerce.BuildingBlocks.EventBus.Events;

namespace ECommerce.BuildingBlocks.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) 
        where T : IntegrationEvent;
    
    void Subscribe<T, THandler>()
        where T : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<T>;
    
    void Unsubscribe<T, THandler>()
        where T : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<T>;
}


