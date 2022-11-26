using System.ComponentModel.DataAnnotations;

namespace TestFoxBe.Dtos;

public class AccomodationDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}

public class AccomodationAddOrUpdDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
}