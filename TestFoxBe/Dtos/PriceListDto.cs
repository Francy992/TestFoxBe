namespace TestFoxBe.Dtos;

public class PriceListDto
{
    public long Id { get; set; }
    public long RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public RoomTypeBaseDto RoomType { get; set; }
}

public class PriceListAddOrUpdateDto
{
    public long RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
}