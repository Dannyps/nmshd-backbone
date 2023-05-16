using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCompleted;

public class RelationshipChangeCompletedIntegrationEventHandler : IIntegrationEventHandler<RelationshipChangeCompletedIntegrationEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RelationshipChangeCompletedIntegrationEventHandler> _logger;

    public RelationshipChangeCompletedIntegrationEventHandler(IUnitOfWork unitOfWork, IEventBus eventBus, ILogger<RelationshipChangeCompletedIntegrationEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(RelationshipChangeCompletedIntegrationEvent integrationEvent)
    {
        await CreateExternalEvent(integrationEvent);
    }

    private async Task CreateExternalEvent(RelationshipChangeCompletedIntegrationEvent integrationEvent)
    {
        var payload = new { integrationEvent.RelationshipId, integrationEvent.ChangeId };
        try
        {
            var owner = integrationEvent.ChangeResult switch
            {
                "Accepted" => integrationEvent.ChangeCreatedBy,
                "Rejected" => integrationEvent.ChangeCreatedBy,
                "Revoked" => integrationEvent.ChangeRecipient,
                _ => throw new ArgumentOutOfRangeException(nameof(integrationEvent.ChangeResult), integrationEvent, null)
            };

            var nextIndex = await _unitOfWork.ExternalEventsRepository.FindNextIndexForIdentity(owner);
            var externalEvent = new ExternalEvent(ExternalEventType.RelationshipChangeCompleted, owner, nextIndex, payload);
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
