using ECommerce.BuildingBlocks.EventBus.Abstractions;
using ECommerce.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ECommerce.BuildingBlocks.EventBus.InMemory;

public class InMemoryEventBus : IEventBus
{
    private readonly ILogger<InMemoryEventBus> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, List<Type>> _handlers;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _handlers = new ConcurrentDictionary<string, List<Type>>();
    }

    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) 
        where T : IntegrationEvent
    {
        var eventName = typeof(T).Name;
        
        _logger.LogInformation("Publishing event {EventName} with ID {EventId}", eventName, integrationEvent.Id);

        if (_handlers.TryGetValue(eventName, out var handlerTypes))
        {
            var tasks = new List<Task>();
            
            foreach (var handlerType in handlerTypes)
            {
                var handler = _serviceProvider.GetService(handlerType);
                if (handler != null)
                {
                    var method = handlerType.GetMethod("HandleAsync");
                    if (method != null)
                    {
                        var task = (Task)method.Invoke(handler, new object[] { integrationEvent, cancellationToken })!;
                        tasks.Add(task);
                    }
                }
            }

            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }
        }

        _logger.LogInformation("Published event {EventName} with ID {EventId}", eventName, integrationEvent.Id);
    }

    public void Subscribe<T, THandler>()
        where T : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(THandler);

        _logger.LogInformation("Subscribing to event {EventName} with handler {HandlerName}", eventName, handlerType.Name);

        _handlers.AddOrUpdate(eventName,
            new List<Type> { handlerType },
            (key, existing) =>
            {
                if (!existing.Contains(handlerType))
                {
                    existing.Add(handlerType);
                }
                return existing;
            });
    }

    public void Unsubscribe<T, THandler>()
        where T : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(THandler);

        _logger.LogInformation("Unsubscribing from event {EventName} with handler {HandlerName}", eventName, handlerType.Name);

        if (_handlers.TryGetValue(eventName, out var handlerTypes))
        {
            handlerTypes.Remove(handlerType);
            if (!handlerTypes.Any())
            {
                _handlers.TryRemove(eventName, out _);
            }
        }
    }
}






