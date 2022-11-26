using Database.Core;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;
public interface IRoomRepository : IRepository<Room, long>
{
    Task<IEnumerable<Room>> FindByAccommodationIdAsync(long accommodationId);
    Task<bool> ExistsByRoomTypeIdAsync(long roomTypeId);
}

public class RoomRepository : Repository<Room, long>, IRoomRepository
{
    public RoomRepository(DbContextAccomodations dbContext) : base(dbContext)
    {
    }

    protected override IQueryable<Room> GetDefaultQuery()
    {
        return DbSet.AsQueryable()
            .Include(x => x.Accomodation)
            .Include(x => x.RoomType)
            .AsNoTracking();
    }

    public async Task<IEnumerable<Room>> FindByAccommodationIdAsync(long accommodationId)
    {
        return await DbSet.AsNoTracking().Where(x => x.AccomodationId == accommodationId).ToListAsync();
    }

    public async Task<bool> ExistsByRoomTypeIdAsync(long roomTypeId)
    {
        return await DbSet.AsNoTracking().AnyAsync(x => x.RoomTypeId == roomTypeId);
    }
}