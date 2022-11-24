using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[Table("Accomodations")]
public class Accomodation : IEntityId<long>
{
    [Key] 		
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Address { get; set; }
    
}