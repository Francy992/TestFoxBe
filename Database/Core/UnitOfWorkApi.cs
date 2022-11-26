using Database.Repositories;
namespace Database.Core;

public interface IUnitOfWorkApi : IUnitOfWork
{
    public IAccomodationRepository AccomodationRepository { get; }
    public IRoomTypeRepository RoomTypeRepository { get; }
    public IPriceListRepository PriceListRepository { get; }
}

public class UnitOfWorkApi : IUnitOfWorkApi
{
    public IAccomodationRepository AccomodationRepository { get; }
    public IRoomTypeRepository RoomTypeRepository { get; }
    public IPriceListRepository PriceListRepository { get; }

    private readonly DbContextAccomodations _dbContext;

    public UnitOfWorkApi(DbContextAccomodations dbContext, IAccomodationRepository accomodationRepository, IRoomTypeRepository roomTypeRepository, IPriceListRepository priceListRepository)
    {
        _dbContext = dbContext;
        AccomodationRepository = accomodationRepository;
        RoomTypeRepository = roomTypeRepository;
        PriceListRepository = priceListRepository;
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
    
}
