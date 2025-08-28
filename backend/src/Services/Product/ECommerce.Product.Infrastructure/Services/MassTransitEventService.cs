using ECommerce.BuildingBlocks.EventBus.Events;
using ECommerce.Product.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ECommerce.Product.Infrastructure.Services;

public class MassTransitEventService : IEventService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitEventService> _logger;

    public MassTransitEventService(IPublishEndpoint publishEndpoint, ILogger<MassTransitEventService> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
    {
        try
        {
            await _publishEndpoint.Publish(@event);
            _logger.LogInformation("Event published successfully: {EventType} with ID: {EventId}", 
                typeof(T).Name, @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event: {EventType} with ID: {EventId}", 
                typeof(T).Name, @event.Id);
            throw;
        }
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T : IntegrationEvent
    {
        try
        {
            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogInformation("Event published successfully: {EventType} with ID: {EventId}", 
                typeof(T).Name, @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event: {EventType} with ID: {EventId}", 
                typeof(T).Name, @event.Id);
            throw;
        }
    }
}
