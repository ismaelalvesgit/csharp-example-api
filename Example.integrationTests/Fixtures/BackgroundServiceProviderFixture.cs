using Example.Application.Modules;
using Example.Application.Services;
using Example.BackgroundTasks.Extensions;
using Example.BackgroundTasks.HostedServices;
using Example.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Example.integrationTests.Fixtures
{
    public class BackgroundServiceProviderFixture
    {
        public readonly IServiceProvider ServiceProvider;

        public BackgroundServiceProviderFixture() {
            IServiceCollection services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            ApplicationModule.Register(services, serviceLifetime: ServiceLifetime.Singleton);
            AddPercistence(services, configuration);
            AddLibs(services);
            AddConsumers(services);
            AddCronJobs(services);
            AddConfigs(services, configuration);
            ServiceProvider = services.BuildServiceProvider();
        }

        private static void AddConfigs(IServiceCollection services, IConfigurationRoot configuration)
        {
            HostingEnvironment env = new() { ApplicationName = "Example.UnitTest" };
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IHostEnvironment>(env);
        }

        private static void AddLibs(IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<ILogger<ProducerService>, Logger<ProducerService>>();
            services.AddAutoMapper(Assembly.Load("Example.Application"));
            services.AddValidatorsFromAssembly(Assembly.Load("Example.Application"));
        }

        private static void AddConsumers(IServiceCollection services)
        {
            services.AddSingleton<ILogger<CategoryConsumer>, Logger<CategoryConsumer>>();
            services.AddConsumer<CategoryConsumer>(c => {
                c.Topic = "Queuing.Example.Category";
            });
        }

        private static void AddCronJobs(IServiceCollection services) 
        {
            services.AddSingleton<ILogger<CategoryJob>, Logger<CategoryJob>>();
            services.AddCronJob<CategoryJob>(c => {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = "* * * * *";
            });
        }

        private static void AddPercistence(IServiceCollection services, IConfigurationRoot configuration) {
            var connectionString = configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
            connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    builder => builder.MigrationsAssembly("Example.Data")), ServiceLifetime.Singleton);
        }
    }
}
