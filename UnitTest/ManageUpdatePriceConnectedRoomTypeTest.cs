using Newtonsoft.Json;
using TestFoxBe.Mediators;
using TestFoxBe.Mocks;
using Xunit;

namespace UnitTest;

public class ManageUpdatePriceConnectedRoomTypeTest : BaseTest
{
    public UpdatePriceConnectedRoomTypeNotifier GetManageUpdatePriceConnectedRoomType()
    {
        return new UpdatePriceConnectedRoomTypeNotifier(UnitOfWorkApi);
    }

    [Fact]
    public async Task TestUpdateOnePriceListUpdateAutomaticallyConnectedPrice()
    {
        var manageUpdatePriceConnectedRoomType = GetManageUpdatePriceConnectedRoomType();
        var price = DbMock.PriceLists.First(x => x.RoomType.RoomTypeIncrementId.HasValue);
        price.Price = 1000;
        UnitOfWorkApi.PriceListRepository.Update(price);
        await UnitOfWorkApi.SaveChanges();
        
        var model = new NotificationMessage()
        {
            NotificationType = NotificationTypeEnum.UpdatePriceConnectedRoomType,
            SerializedObj = JsonConvert.SerializeObject(new UpdatePriceConnectedRoomTypeDto()
            {
                Date = price.Date,
                RoomTypeId = price.RoomTypeId
            })
        };
        
        await manageUpdatePriceConnectedRoomType.Handle(model, new CancellationToken());

        var result = await UnitOfWorkApi.PriceListRepository.FindAll();
        var toUpdated = result.Where(x => x.RoomType.RoomTypeIncrementId == price.RoomTypeId && x.Date == price.Date).ToList();
        Assert.NotNull(toUpdated);
        foreach (var update in toUpdated)
        {
            var original = DbMock.PriceLists.First(x => x.Id == update.Id);
            Assert.NotEqual(original.Price, update.Price);
        }
    }
}