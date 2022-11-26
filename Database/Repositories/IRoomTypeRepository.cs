using Database.Core;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;
public interface IRoomTypeRepository : IRepository<RoomType, long>
{
    Task<bool> ExistsByRoomTypeIdAsync(long roomTypeId);
    Task<IEnumerable<RoomType>> FindByRoomTypeIncrementIdAsync(long roomTypeIncrementId);
}

public class RoomTypeRepository : Repository<RoomType, long>, IRoomTypeRepository
{
    public RoomTypeRepository(DbContextAccomodations dbContext) : base(dbContext)
    {
    }

    protected override IQueryable<RoomType> GetDefaultQuery()
    {
        return DbSet.AsQueryable()
            .Include(x => x.RoomTypeIncrement)
            .AsNoTracking();
    }

    public async Task<bool> ExistsByRoomTypeIdAsync(long roomTypeId)
    {
        return await DbSet.AsNoTracking().AnyAsync(x => x.RoomTypeIncrementId.HasValue && x.RoomTypeIncrementId.Value == roomTypeId);
    }

    public async Task<IEnumerable<RoomType>> FindByRoomTypeIncrementIdAsync(long roomTypeIncrementId)
    {
        return await GetDefaultQuery().Where(x => x.RoomTypeIncrementId.HasValue && x.RoomTypeIncrementId.Value == roomTypeIncrementId).ToListAsync();
    }
}