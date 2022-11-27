using System.Threading.Tasks;
using Database.Core;
using Database.Repositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using TestFoxBe.Mediators;
using TestFoxBe.Mocks;
using UnitTest.InMemoryDb;

namespace UnitTest;

public class BaseTest
{
    protected readonly DbContextAccomodations DbContextAccomodations;
    protected readonly IUnitOfWorkApi UnitOfWorkApi;

    public BaseTest()
    {
        DbContextAccomodations = GetDbContext().Result;
        UnitOfWorkApi = GetUnitOfWorkProduct(DbContextAccomodations);
    }
    
    private IUnitOfWorkApi GetUnitOfWorkProduct(DbContextAccomodations dbContextAccomodations)
    {
        return new UnitOfWorkApi(dbContextAccomodations,
            new AccomodationRepository(dbContextAccomodations), 
            new RoomTypeRepository(dbContextAccomodations), 
            new PriceListRepository(dbContextAccomodations));
    }

    protected async Task<DbContextAccomodations> GetDbContext()
    {
        var options = SqliteInMemory.CreateOptions<DbContextAccomodations>();
        var context = new DbContextAccomodations(options);
        await context.Database.EnsureCreatedAsync();
        await SeedDatabase(context);
        DetachAllEntities(context);


        TypeAdapterConfig.GlobalSettings.Default
            .ShallowCopyForSameType(true)
            .MaxDepth(5);
        return context;
    }
    
    private async Task SeedDatabase(DbContextAccomodations context)
    {
        context.Accomodations.AddRange(DbMock.Accomodations);
        context.RoomTypes.AddRange(DbMock.RoomTypes);
        context.PriceLists.AddRange(DbMock.PriceLists);
        await context.SaveChangesAsync();    
    }
    
    protected void DetachAllEntities<T>(T context) where T : DbContext
    {
        foreach (var dbEntityEntry in context.ChangeTracker.Entries())
        {
            if (dbEntityEntry.Entity != null)
            {
                dbEntityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }
    }
    
    protected static Mock<INotifierMediatorService> GetNotifierMediatorServiceMock()
    {
        var notifierMediatorServiceMock = new Mock<INotifierMediatorService>();
        notifierMediatorServiceMock.Setup(x => x.Notify(It.IsAny<NotificationTypeEnum>(), It.IsAny<object>())).Verifiable();
        return notifierMediatorServiceMock;
    }
}