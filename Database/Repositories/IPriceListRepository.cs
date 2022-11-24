using Database.Core;
using Database.Models;

namespace Database.Repositories;
public interface IPriceListRepository : IRepository<PriceList, long>
{
}

public class PriceListRepository : Repository<PriceList, long>, IPriceListRepository
{
    public PriceListRepository(DbContextAccomodations dbContext) : base(dbContext)
    {
    }
}