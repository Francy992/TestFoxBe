using Database.Core;
using Database.Models;

namespace Database.Repositories;
public interface IRoomTypeRepository : IRepository<RoomType, long>
{
}

public class RoomTypeRepository : Repository<RoomType, long>, IRoomTypeRepository
{
    public RoomTypeRepository(DbContextAccomodations dbContext) : base(dbContext)
    {
    }
}