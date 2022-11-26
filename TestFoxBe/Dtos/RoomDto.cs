using System.ComponentModel.DataAnnotations;

namespace TestFoxBe.Dtos;

public class RoomDto
{
    public string Name { get; set; }
    public long AccomodationId { get; set; }
    public long RoomTypeId { get; set; }
    public AccomodationDto Accomodation { get; set; }
    public RoomTypeBaseDto RoomType { get; set; }
}

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

public class RoomTypeAddOrUpdDto
{
    public string Name { get; set; }
}