using Example.Application.Modules;
using Example.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Example.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static WebApplicationBuilder AddPersistense(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                builder => builder.MigrationsAssembly("Example.Data")));

        return builder;
    }

    public static WebApplicationBuilder AddApplicationModule(this WebApplicationBuilder builder)
    {
        ApplicationModule.Register(builder.Services);

        return builder;
    }
}
