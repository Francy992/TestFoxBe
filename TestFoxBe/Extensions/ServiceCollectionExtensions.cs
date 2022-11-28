using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace TestFoxBe.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void SetSwaggerInfo(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Accomodations API",
                Version = "v1",
                Description = "TestFoxBe API",
                Contact = new OpenApiContact
                {
                    Name = "Francesco Anastasio",
                    Email = "francesco.anastasio.92@gmail.com",
                }
            }); 
            var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            c.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
            c.TagActionsBy(api => new List<string> {api.GroupName});
            c.OrderActionsBy(x => x.GroupName);
        });
    }

    public static void SetCorsPolicy(this IServiceCollection services, string corsPolicyName)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }
}