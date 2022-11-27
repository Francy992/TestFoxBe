using MediatR;

namespace TestFoxBe.Mediators;

public class NotificationMessage : INotification
{
    public NotificationTypeEnum NotificationType { get; set; }
    public string SerializedObj { get; set; }
}

public enum NotificationTypeEnum
{
    UpdatePriceConnectedRoomType
}

public class UpdatePriceConnectedRoomTypeDto
{
    public long RoomTypeId { get; set; }
    public decimal Price { get; set; }
    public DateTime? Date { get; set; }
}