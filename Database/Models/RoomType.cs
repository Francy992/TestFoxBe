using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[Table("RoomTypes")]
public class RoomType : IEntityId<long>
{
    [Key] 		
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; }
    
    [ForeignKey(nameof(RoomType))]
    public long? RoomTypeIncrementId { get; set; }
    
    public decimal? PriceIncrementPercentage { get; set; }
    
    // Navigation properties
    public virtual RoomType? RoomTypeIncrement { get; set; }
}