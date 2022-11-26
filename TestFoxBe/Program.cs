using Database.Core;
using Database.Repositories;
using Microsoft.EntityFrameworkCore;
using TestFoxBe.Extensions;
using TestFoxBe.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DbContextAccomodations>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// cors
const string corsPolicyName = "AllowAll";
builder.Services.SetCorsPolicy(corsPolicyName);

// Add services
builder.Services.AddScoped<IAccomodationRepository, AccomodationRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IPriceListRepository, PriceListRepository>();
builder.Services.AddScoped<IUnitOfWorkApi, UnitOfWorkApi>();

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

static void MigrateDatabase(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DbContextAccomodations>();
        context.Database.Migrate();
        // TODO: add initial data if not exists
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}