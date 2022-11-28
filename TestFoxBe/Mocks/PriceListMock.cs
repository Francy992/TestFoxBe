using Database.Models;

namespace TestFoxBe.Mocks;

public partial class DbMock
{
    public static List<PriceList> PriceLists = new()
    {
        new PriceList() { Id = 1, Date = new DateTime(2021, 1, 1), RoomTypeId = 1, Price = 100 }, // Single room
        new PriceList() { Id = 5, Date = new DateTime(2021, 1, 1), RoomTypeId = 2, Price = 111 }, // Single room
        new PriceList() { Id = 2, Date = new DateTime(2021, 1, 1), RoomTypeId = 2, Price = 110 }, // Double room
        new PriceList() { Id = 3, Date = new DateTime(2021, 1, 1), RoomTypeId = 3, Price = 132 }, // Suite
        new PriceList() { Id = 4, Date = new DateTime(2021, 1, 1), RoomTypeId = 4, Price = 172 }, // Deluxe
    };
}