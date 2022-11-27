using Database.Models;

namespace TestFoxBe.Mocks;

public partial class DbMock
{
    public static List<RoomType> RoomTypes = new()
    {
        new RoomType() { Id = 1, Name = "Single", RoomTypeIncrementId = null, PriceIncrementPercentage = null },
        new RoomType() { Id = 2, Name = "Double", RoomTypeIncrementId = 1, PriceIncrementPercentage = 10 },
        new RoomType() { Id = 3, Name = "Suite", RoomTypeIncrementId = 2, PriceIncrementPercentage = 20 },
        new RoomType() { Id = 4, Name = "Deluxe", RoomTypeIncrementId = 3, PriceIncrementPercentage = 30 },
    };
}