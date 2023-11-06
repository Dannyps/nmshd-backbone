namespace Backbone.Modules.Devices.Domain.Entities;

public class SystemNotification
{
    public SystemNotification(string message)
    {
        Id = SystemNotificationId.New();
        Message = message;
    }

    public SystemNotification(string message, DateTime validFrom, DateTime validTo)
    {
        Id = SystemNotificationId.New();
        Message = message;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public SystemNotificationId Id { get; set; }
    public string Message { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
