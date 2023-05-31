using Example.BackgroundTasks.Interfaces;
using Example.BackgroundTasks.Services;
using Example.Domain.Entitys;
using Example.Domain.Interfaces.Services;

namespace Example.BackgroundTasks.HostedServices
{
    public class CategoryJob : CronJobServiceBase
    {
        private readonly ILogger<CategoryJob> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryJob(
            IScheduleConfig<CategoryJob> config,
            ILogger<CategoryJob> logger,
            ICategoryService categoryService) : base(config.CronExpression, config.TimeZoneInfo, logger)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CategoryJob starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var time = $"{DateTime.Now:hh: mm: ss}";
            _logger.LogInformation("{time} CategoryJob is working.", time);
            var category = new Category()
            {
                Name = $"Ismael {DateTime.Now}",
                ImageUrl = "https://ismael.alves.com"
            };
            await _categoryService.InsertAsync(category);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CategoryJob is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
