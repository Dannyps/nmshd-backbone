using Backbone.Modules.Synchronization.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.MessageCreated;

public class MessageCreatedIntegrationEventHandler : IIntegrationEventHandler<MessageCreatedIntegrationEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;
    private readonly ILogger<MessageCreatedIntegrationEventHandler> _logger;

    public MessageCreatedIntegrationEventHandler(IUnitOfWork unitOfWork, IEventBus eventBus, ILogger<MessageCreatedIntegrationEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(MessageCreatedIntegrationEvent integrationEvent)
    {
        await CreateExternalEvents(integrationEvent);
    }

    private async Task CreateExternalEvents(MessageCreatedIntegrationEvent integrationEvent)
    {
        foreach (var recipient in integrationEvent.Recipients)
        {
            var payload = new { integrationEvent.Id };
            try
            {
                var owner = IdentityAddress.Parse(recipient);
                var nextIndex = await _unitOfWork.ExternalEventsRepository.FindNextIndexForIdentity(owner);
                var externalEvent = new ExternalEvent(ExternalEventType.MessageReceived, owner, nextIndex, payload);
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
}
