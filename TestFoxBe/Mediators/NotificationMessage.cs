using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace TestFoxBe.Mediators;

[ExcludeFromCodeCoverage]
public class NotificationMessage : INotification
{
    public NotificationTypeEnum NotificationType { get; set; }
    public string SerializedObj { get; set; }
}

public enum NotificationTypeEnum
{
    UpdatePriceConnectedRoomType
}

[ExcludeFromCodeCoverage]
public class UpdatePriceConnectedRoomTypeDto
{
    public long RoomTypeId { get; set; }
    public DateTime? Date { get; set; }
}