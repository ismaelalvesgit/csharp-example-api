using Example.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Example.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder UseContextMigrations(this IApplicationBuilder app)
    {
        var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<AppDbContext>();

        if (context != null && context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory" && context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        return app;
    }

    public static IApplicationBuilder UseSwaggerSetup(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Example.Services.Api v1");
        });

        return app;
    }
}
