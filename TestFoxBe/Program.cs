using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Database.Core;
using Database.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TestFoxBe.Extensions;
using TestFoxBe.Mediators;
using TestFoxBe.Middlewares;
using TestFoxBe.Mocks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DbContextAccomodations>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// cors
const string corsPolicyName = "AllowAll";
builder.Services.SetCorsPolicy(corsPolicyName);

// Add services
builder.Services.AddScoped<IAccomodationRepository, AccomodationRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IPriceListRepository, PriceListRepository>();
builder.Services.AddScoped<IUnitOfWorkApi, UnitOfWorkApi>();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddTransient<INotifierMediatorService, NotifierMediatorService>();

builder.Services.AddControllers();
builder.Services.SetSwaggerInfo();

var app = builder.Build();

MigrateDatabase(app);


app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(corsPolicyName);
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();
app.Run();

[ExcludeFromCodeCoverage]
static void MigrateDatabase(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;

    Task.Delay(20000).Wait();
    try
    {
        var context = services.GetRequiredService<DbContextAccomodations>();
        context.Database.Migrate();

        var element = context.Accomodations.FirstOrDefault();
        if (element == null)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Accomodations ON;");
                context.Accomodations.AddRange(DbMock.Accomodations);
                context.SaveChanges();
                context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Accomodations OFF;");

                context.RoomTypes.AddRange(DbMock.RoomTypes);
                context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT RoomTypes ON;");
                context.SaveChanges();
                context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT RoomTypes OFF;");

                context.PriceLists.AddRange(DbMock.PriceLists);
                context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT PriceLists ON;");
                context.SaveChanges();
                context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT PriceLists OFF;");
                transaction.Commit();
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}


[ExcludeFromCodeCoverage]
public partial class Program { }