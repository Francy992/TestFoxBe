using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[Table("Rooms")]
public class Room : IEntityId<long>
{
    [Key] 		
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; }
    
    [Required]
    [ForeignKey(nameof(Accomodation))]
    public long AccomodationId { get; set; }
    
    [Required]
    [ForeignKey(nameof(RoomType))]
    public long RoomTypeId { get; set; }
    
    // Navigation properties
    public Accomodation Accomodation { get; set; }
    public RoomType RoomType { get; set; }
}