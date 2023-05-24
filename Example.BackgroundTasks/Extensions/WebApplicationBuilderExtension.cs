using Example.Application.Modules;
using Example.BackgroundTasks.HostedServices;
using Example.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Example.BackgroundTasks.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddPersistense(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                builder => builder.MigrationsAssembly("Example.Data")), ServiceLifetime.Singleton);

        return builder;
    }

    public static WebApplicationBuilder AddApplicationModule(this WebApplicationBuilder builder)
    {
        ApplicationModule.Register(builder.Services, serviceLifetime: ServiceLifetime.Singleton);

        return builder;
    }

    public static WebApplicationBuilder AddCronJobs(this WebApplicationBuilder builder)
    {
        var jobs = builder.Configuration.GetSection("Jobs").Get<Dictionary<string, string>>();
        
        builder.Services.AddCronJob<CategoryJob>(c =>
        {
            c.TimeZoneInfo = TimeZoneInfo.Local;
            c.CronExpression = $@"{jobs["CategoryJob"]}";
        });

        return builder;
    }

    public static WebApplicationBuilder AddConsumers(this WebApplicationBuilder builder)
    {
        builder.Services.AddConsumer<CategoryConsumer>(c =>
        {
            c.topic = "Queuing.Example.Category";
        });

        return builder;
    }

    public static WebApplicationBuilder AddLibs(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(Assembly.Load("Example.Application"));
        builder.Services.AddValidatorsFromAssembly(Assembly.Load("Example.Application"));

        return builder;
    }   
}
