using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Classes;

namespace Backbone.Modules.Devices.Domain.Entities;

public class SystemNotificationId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;

    private const string PREFIX = "SNI";

    private SystemNotificationId(string stringValue) : base(stringValue) { }

    public static SystemNotificationId New()
    {
        var devicePushIdentifierIdAsString = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new SystemNotificationId(PREFIX + devicePushIdentifierIdAsString);
    }

    public static SystemNotificationId Parse(string stringValue)
    {
        return new SystemNotificationId(stringValue);
    }
}
