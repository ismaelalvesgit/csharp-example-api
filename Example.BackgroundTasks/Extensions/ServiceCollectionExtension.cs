using Example.BackgroundTasks.Interfaces;
using Example.BackgroundTasks.Model;
using Example.BackgroundTasks.Services;

namespace Example.BackgroundTasks.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobServiceBase
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
            }
            var config = new ScheduleConfig<T>();
            options.Invoke(config);

            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }

        public static IServiceCollection AddConsumer<T>(this IServiceCollection services, Action<IConsumerConfig<T>> options) where T : ConsumerBaseService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), @"Please provide Consumer Configurations.");
            }

            var config = new ConsumerConfig<T>();
            options.Invoke(config);

            if (string.IsNullOrWhiteSpace(config.Topic))
            {
                throw new ArgumentNullException(nameof(T), @"Please provide valid data from Consumer");
            }

            services.AddSingleton<IConsumerConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
