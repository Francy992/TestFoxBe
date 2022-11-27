using Database.Core;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;
public interface IPriceListRepository : IRepository<PriceList, long>
{
    Task<bool> ExistsByRoomTypeIdAsync(long roomTypeId);
    Task<IEnumerable<PriceList>> GetByRoomTypeAndDateAsync(long roomTypeId, DateTime? date);
    Task<decimal> GetMaxPriceByRoomTypeAndDateAsync(long roomTypeId, DateTime? date);
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

    public async Task<IEnumerable<PriceList>> GetByRoomTypeAndDateAsync(long roomTypeId, DateTime? date)
    {
        return await GetDefaultQuery().Where(x => x.RoomTypeId == roomTypeId && (!date.HasValue || x.Date.Date == date.Value.Date)).ToListAsync();
    }

    public async Task<decimal> GetMaxPriceByRoomTypeAndDateAsync(long roomTypeId, DateTime? date)
    {
        var tmp =  await DbSet.AsNoTracking().Where(x => x.RoomTypeId == roomTypeId && (!date.HasValue || x.Date.Date == date.Value.Date)).ToListAsync();
        return tmp.Count > 0 ? tmp.Max(x => x.Price) : 0;
    }
}