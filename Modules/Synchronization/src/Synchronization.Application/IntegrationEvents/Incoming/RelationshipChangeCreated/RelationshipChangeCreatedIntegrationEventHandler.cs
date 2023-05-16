using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCreated;

public class RelationshipChangeCreatedIntegrationEventHandler : IIntegrationEventHandler<RelationshipChangeCreatedIntegrationEvent>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<RelationshipChangeCreatedIntegrationEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RelationshipChangeCreatedIntegrationEventHandler(IUnitOfWork unitOfWork, IEventBus eventBus, ILogger<RelationshipChangeCreatedIntegrationEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipChangeCreatedIntegrationEvent integrationEvent)
    {
        await CreateExternalEvent(integrationEvent);
    }

    private async Task CreateExternalEvent(RelationshipChangeCreatedIntegrationEvent integrationEvent)
    {
        var payload = new { integrationEvent.RelationshipId, integrationEvent.ChangeId };
        try
        {
            var nextIndex = await _unitOfWork.ExternalEventsRepository.FindNextIndexForIdentity(integrationEvent.ChangeRecipient);
            var externalEvent = new ExternalEvent(ExternalEventType.RelationshipChangeCreated, integrationEvent.ChangeRecipient, nextIndex, payload);
            _unitOfWork.ExternalEventsRepository.Add(externalEvent);

            await _unitOfWork.Save(CancellationToken.None);

            _eventBus.Publish(new ExternalEventCreatedIntegrationEvent(externalEvent));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing an integration event.");
            throw;
        }
    }
}
