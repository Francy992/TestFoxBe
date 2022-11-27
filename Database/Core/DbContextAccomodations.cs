using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Core;

public class DbContextAccomodations: DbContext
{
    public DbContextAccomodations(DbContextOptions<DbContextAccomodations> options)
        : base(options)
    {
    }

    public DbSet<Accomodation> Accomodations { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<PriceList> PriceLists { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
    }
}