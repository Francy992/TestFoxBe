using Database.Core;
using Database.Models;

namespace Database.Repositories;
public interface IAccomodationRepository : IRepository<Accomodation, long>
{
}

public class AccomodationRepository : Repository<Accomodation, long>, IAccomodationRepository
{
    public AccomodationRepository(DbContextAccomodations dbContext) : base(dbContext)
    {
    }
}