using System.ComponentModel.DataAnnotations;

namespace TestFoxBe.Dtos;

public class RoomAddOrUpdDto
{
    public string Name { get; set; }
    public long AccomodationId { get; set; }
    public long RoomTypeId { get; set; }
}

public class RoomTypeBaseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
}

public class RoomTypeDto : RoomTypeBaseDto
{
    public RoomTypeBaseDto RoomTypeIncrement { get; set; }
    public decimal? PriceIncrementPercentage { get; set; }
}

public class RoomTypeAddOrUpdDto
{
    public string Name { get; set; }
    public long? RoomTypeIncrementId { get; set; }
    public decimal? PriceIncrementPercentage { get; set; }
}