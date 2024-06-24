﻿using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class RelationshipReactivationCompletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipReactivationCompletedIntegrationEvent = new RelationshipReactivationCompletedDomainEvent("someRelationshipId", identityAddress);

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.RelationshipReactivationCompleted, IdentityAddress.Parse(identityAddress), 1,
            new { relationshipReactivationCompletedIntegrationEvent.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            identityAddress,
            ExternalEventType.RelationshipReactivationCompleted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new RelationshipReactivationCompletedDomainEventHandler(mockDbContext,
            A.Fake<ILogger<RelationshipReactivationCompletedDomainEventHandler>>());

        // Act
        await handler.Handle(relationshipReactivationCompletedIntegrationEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(identityAddress, ExternalEventType.RelationshipReactivationCompleted, A<object>._))
            .MustHaveHappenedOnceExactly();
    }
}
