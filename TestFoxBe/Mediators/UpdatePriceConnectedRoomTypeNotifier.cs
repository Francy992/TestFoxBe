using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Database.Core;
using MediatR;
using Newtonsoft.Json;

namespace TestFoxBe.Mediators;

[ExcludeFromCodeCoverage]
public class UpdatePriceConnectedRoomTypeNotifier : INotificationHandler<NotificationMessage>
{
    private readonly IUnitOfWorkApi _unitOfWork;

    public UpdatePriceConnectedRoomTypeNotifier(IUnitOfWorkApi unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(NotificationMessage notification, CancellationToken cancellationToken)
    {
        switch (notification.NotificationType)
        {
            case NotificationTypeEnum.UpdatePriceConnectedRoomType:
                await ManageUpdatePriceConnectedRoomType(notification);
                break;
            default:
                throw new Exception("Notification type not supported");
        }
        
        // return Task.CompletedTask;
    }

    /// <summary>
    /// Check, if there are other price list connected with current room type and update them
    /// For do this, I check if there are other room type connected with current room type and I update their price list
    /// I use queue to simulate recursion
    /// </summary>
    private async Task ManageUpdatePriceConnectedRoomType(NotificationMessage notification)
    {
        var deserialized = JsonConvert.DeserializeObject<UpdatePriceConnectedRoomTypeDto>(notification.SerializedObj);
        if(deserialized == null)
            throw new Exception("Error deserializing object");

        var roomTypeQueue = new Queue<long>();
        roomTypeQueue.Enqueue(deserialized.RoomTypeId);
        do
        {
            var currentRoomTypeId = roomTypeQueue.Dequeue();
            // Find the room type that have "RoomTypeIncrementId" equal to "currentRoomTypeId"
            var roomTypeToCheck = await _unitOfWork.RoomTypeRepository.FindByRoomTypeIncrementIdAsync(currentRoomTypeId);
            // Find max price list connected with current room type. This price list will be used to update other price list connected with current room type
            var referencePrice = await _unitOfWork.PriceListRepository.GetMaxPriceByRoomTypeAndDateAsync(currentRoomTypeId, deserialized.Date);
            foreach(var roomTypeToCheckIncrement in roomTypeToCheck)
            {
                var priceListToCheckIncrement = await _unitOfWork.PriceListRepository.GetByRoomTypeAndDateAsync(roomTypeToCheckIncrement.Id, deserialized.Date);
                foreach(var priceListToIncrement in priceListToCheckIncrement)
                {
                    var minPrice = referencePrice + (referencePrice * roomTypeToCheckIncrement.PriceIncrementPercentage!.Value / 100);
                    if (priceListToIncrement.Price < minPrice)
                    {
                        priceListToIncrement.RoomType = null;
                        priceListToIncrement.Price = minPrice;
                        _unitOfWork.PriceListRepository.Update(priceListToIncrement);
                        if(!roomTypeQueue.Contains(priceListToIncrement.RoomTypeId))
                            roomTypeQueue.Enqueue(priceListToIncrement.RoomTypeId);
                    }
                }
            }
            await _unitOfWork.SaveChanges();
        }
        while(roomTypeQueue.Count > 0);
    }
}