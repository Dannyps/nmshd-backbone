using Backbone.Modules.Devices.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests;
public class SystemNotificationTests
{
    [Fact]
    public void Should_create_SystemNotification_without_time_parameters()
    {
        // Arrange

        // Act
        var notification = new SystemNotification("");

        // Assert
        notification.Id.Should().BeOfType<SystemNotificationId>();
        notification.Id.StringValue[..3].Should().Be("SNI");
        notification.Id.StringValue.Length.Should().Be(20);

        notification.Message.Should().NotBeNull();

        notification.ValidFrom.Should().BeNull();
        notification.ValidTo.Should().BeNull();
    }

    [Fact(Skip = "not implemeted")]
    public void Should_create_SystemNotification_with_time_parameters() { }

    [Fact(Skip = "not implemeted")]
    public void Should_create_SystemNotification_with_ValidFrom_parameter() { }

    [Fact(Skip ="not implemeted")]
    public void Should_create_SystemNotification_with_ValidTo_parameter() { }
}
