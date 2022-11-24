using Database.Core;
using Database.Models;

namespace Database.Repositories;
public interface IRoomRepository : IRepository<Room, long>
{
}

public class RoomRepository : Repository<Room, long>, IRoomRepository
{
    public RoomRepository(DbContextAccomodations dbContext) : base(dbContext)
    {
    }
}