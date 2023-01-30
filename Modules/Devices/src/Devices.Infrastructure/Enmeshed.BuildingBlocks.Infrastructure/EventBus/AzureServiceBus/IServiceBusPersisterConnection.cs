﻿using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace Backbone.Modules.Devices.Infrastructure.Enmeshed.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus
{
    public interface IServiceBusPersisterConnection : IDisposable
    {
        ServiceBusClient TopicClient { get; }
        ServiceBusAdministrationClient AdministrationClient { get; }
    }
}
