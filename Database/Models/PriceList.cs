using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[Table("PriceLists")]
public class PriceList : IEntityId<long>
{
    [Key] 		
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [ForeignKey(nameof(RoomType))]
    public long RoomTypeId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    // Navigation properties
    public RoomType? RoomTypeIncrement { get; set; }
}