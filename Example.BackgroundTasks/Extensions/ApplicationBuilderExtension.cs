using Example.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Example.BackgroundTasks.Extensions;

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
}
