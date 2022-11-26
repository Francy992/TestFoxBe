using Database.Core;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;
public interface IPriceListRepository : IRepository<PriceList, long>
{
    Task<bool> ExistsByRoomTypeIdAsync(long roomTypeId);
}

public class PriceListRepository : Repository<PriceList, long>, IPriceListRepository
{
    public PriceListRepository(DbContextAccomodations dbContext) : base(dbContext)
    {
    }

    protected override IQueryable<PriceList> GetDefaultQuery()
    {
        return DbSet.AsQueryable()
            .Include(x => x.RoomType)
            .AsNoTracking();
    }

    public async Task<bool> ExistsByRoomTypeIdAsync(long roomTypeId)
    {
        return await DbSet.AsNoTracking().AnyAsync(x => x.RoomTypeId == roomTypeId);
    }
}